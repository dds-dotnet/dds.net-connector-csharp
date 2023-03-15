using DDS.Net.Connector.Interfaces.SyncQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public NetworkClient()
        {
            dataToServerQueue = new(1000);
            dataFromServerQueue = new(1000);
        }

        public void Connect(string serverIPv4, ushort portTCP)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public ISyncQueueReader<PacketFromServer> GetDataQueueFromServer()
        {
            throw new NotImplementedException();
        }

        public ISyncQueueWriter<PacketToServer> GetDataQueueToServer()
        {
            throw new NotImplementedException();
        }
    }
}
