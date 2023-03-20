using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces.SyncQueue;
using System.Net;
using System.Net.Sockets;

namespace DDS.Net.Connector.Interfaces.NetworkClient
{
    /// <summary>
    /// Class <c>NetworkClient</c> implements <c>IThreadedNetworkClient</c> interface,
    /// and provides connectivity with the server.
    /// </summary>
    internal class NetworkClient : IThreadedNetworkClient
    {
        private SyncQueue<PacketToServer> dataToServerQueue;
        private SyncQueue<PacketFromServer> dataFromServerQueue;

        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <param name="dataToServerQueueSize">Total number of
        ///   <c cref="PacketToServer">PacketToServer</c>
        ///   objects that the queue can hold.</param>
        /// <param name="dataFromServerQueueSize">Total number of
        ///   <c cref="PacketFromServer">PacketFromServer</c>
        ///   objects that the queue can hold.</param>
        public NetworkClient(int dataToServerQueueSize = 1000, int dataFromServerQueueSize = 1000)
        {
            dataToServerQueue = new(dataToServerQueueSize);
            dataFromServerQueue = new(dataFromServerQueueSize);
        }

        private volatile bool isIOThreadStarted = false;
        private Thread ioThread = null!;
        private Socket socket = null!;
        private IPEndPoint targetEndPoint = null!;

        public event Action? ConnectedWithServer;
        public event Action? DisonnectedFromServer;

        public void Connect(string serverIPv4, ushort portTCP)
        {
            lock (this)
            {
                if (isIOThreadStarted)
                {
                    throw new Exception($"Connection routine has already been started");
                }
                else
                {
                    if (serverIPv4.IsValidIPv4Address())
                    {
                        targetEndPoint = new IPEndPoint(IPAddress.Parse(serverIPv4), portTCP);
                    }
                    else
                    {
                        throw new Exception($"Invalid IPv4 address: {serverIPv4}");
                    }
                    
                    isIOThreadStarted = true;
                    ioThread = new Thread(IOThreadFunc);
                    ioThread.Start();
                }
            }
        }

        private void IOThreadFunc(object? obj)
        {
            while (isIOThreadStarted)
            {
                if (socket == null)
                {
                    try
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                        {
                            Blocking = false
                        };
                    }
                    catch (Exception)
                    {
                        try
                        {
                            socket?.Close();
                            socket?.Dispose();
                        }
                        catch { }

                        socket = null!;
                    }
                }
                else
                {
                    if (!socket.Connected)
                    {
                        try
                        {
                            socket.ConnectAsync(targetEndPoint);
                            ConnectedWithServer?.Invoke();
                        }
                        catch
                        {
                            Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        bool doneAnythingInIteration = false;

                        try
                        {
                            //- 
                            //- Receiving data
                            //- 

                            if (socket.Available > 0)
                            {
                                doneAnythingInIteration = true;

                                byte[] bytes = new byte[socket.Available];

                                int totalReceived = socket.Receive(bytes, SocketFlags.None);

                                dataFromServerQueue.Enqueue(new PacketFromServer(bytes));
                            }

                            //- 
                            //- Transmitting data
                            //- 

                            while (dataToServerQueue.CanDequeue())
                            {
                                doneAnythingInIteration = true;

                                PacketToServer packet = dataToServerQueue.Dequeue();

                                socket.Send(packet.Data, packet.TotalBytesToBeSent, SocketFlags.None);
                            }
                        }
                        catch
                        {
                            DisonnectedFromServer?.Invoke();

                            try
                            {
                                socket?.Close();
                                socket?.Dispose();
                            }
                            catch { }

                            socket = null!;
                        }

                        if (!doneAnythingInIteration)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
            } // while (isIOThreadStarted)

            try
            {
                socket?.DisconnectAsync(false);
            }
            catch { }

            try
            {
                socket?.Close();
                socket?.Dispose();
            }
            catch { }

            socket = null!;
        }

        public void Disconnect()
        {
            lock (this)
            {
                if (isIOThreadStarted == true)
                {
                    try
                    {
                        isIOThreadStarted = false;

                        ioThread?.Join(500);
                        ioThread = null!;
                    }
                    catch { }
                }
            }
        }

        public ISyncQueueReader<PacketFromServer> GetDataQueueFromServer()
        {
            return dataFromServerQueue;
        }

        public ISyncQueueWriter<PacketToServer> GetDataQueueToServer()
        {
            return dataToServerQueue;
        }
    }
}
