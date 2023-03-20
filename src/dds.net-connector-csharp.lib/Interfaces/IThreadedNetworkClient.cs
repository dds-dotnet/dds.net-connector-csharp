using DDS.Net.Connector.Interfaces.NetworkClient;

namespace DDS.Net.Connector.Interfaces
{
    /// <summary>
    /// Interface <c>IThreadedNetworkClient</c> represents a network client.
    /// </summary>
    internal interface IThreadedNetworkClient
    {
        /// <summary>
        /// The event is invoked when client connects with the server.
        /// </summary>
        public event Action? ConnectedWithServer;
        /// <summary>
        /// The event is invoked when client disconnects from the server.
        /// </summary>
        public event Action? DisconnectedFromServer;

        /// <summary>
        /// Connects with specified server.
        /// </summary>
        /// <param name="serverIPv4">Address of target server in IP v4 format.</param>
        /// <param name="portTCP">Target server's TCP port number.</param>
        void Connect(string serverIPv4, ushort portTCP);
        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Gets data queue's end for reading data packets received from the server.
        /// </summary>
        /// <returns>Data queue's reader interface.</returns>
        ISyncQueueReader<PacketFromServer> GetDataQueueFromServer();
        /// <summary>
        /// Gets data queue's end for writing data packets that need to be sent to the server.
        /// </summary>
        /// <returns>Data queue's writer interface.</returns>
        ISyncQueueWriter<PacketToServer> GetDataQueueToServer();
    }
}
