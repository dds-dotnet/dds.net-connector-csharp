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

        public void Connect(string serverIPv4, ushort portTCP)
        {
            if (serverIPv4.IsValidIPv4Address())
            {
                lock (this)
                {
                    if (isIOThreadStarted == false)
                    {
                        try
                        {
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                            {
                                Blocking = false
                            };

                            socket.ConnectAsync(
                                new IPEndPoint(IPAddress.Parse(serverIPv4), portTCP));

                            isIOThreadStarted = true;
                            ioThread = new Thread(IOThreadFunc);
                            ioThread.Start();
                        }
                        catch (Exception ex)
                        {
                            socket?.Close();
                            socket?.Dispose();
                            socket = null!;

                            isIOThreadStarted = false;
                            ioThread = null!;

                            throw new Exception($"Cannot connect with the server {serverIPv4}:{portTCP} - {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Invalid IPv4 address: {serverIPv4}");
            }
        }

        private void IOThreadFunc(object? obj)
        {
            while (isIOThreadStarted)
            {
                bool doneAnythingInIteration = false;

                try
                {
                    //- 
                    //- Receiving data
                    //- 

                    int dataAvailable = socket.Available;

                    if (dataAvailable > 0)
                    {
                        doneAnythingInIteration = true;

                        byte[] data = new byte[dataAvailable];
                        socket.Receive(data);

                        dataFromServerQueue.Enqueue(new PacketFromServer(data));
                    }

                    //- 
                    //- Transmitting data
                    //- 

                    if (dataToServerQueue.CanDequeue())
                    {
                        doneAnythingInIteration = true;

                        PacketToServer packet = dataToServerQueue.Dequeue();

                        socket.Send(packet.Data, packet.TotalBytesToBeSent, SocketFlags.None);
                    }
                }
                catch { }

                if (!doneAnythingInIteration)
                {
                    Thread.Sleep(10);
                }
            }
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

                        ioThread?.Join(200);
                        ioThread = null!;
                    }
                    catch { }

                    try
                    {
                        socket.DisconnectAsync(false);
                    }
                    catch { }

                    try
                    {
                        socket.Close();
                        socket.Dispose();
                        socket = null!;
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
