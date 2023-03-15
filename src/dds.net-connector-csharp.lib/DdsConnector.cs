using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDS.Net.Connector
{
    public class DdsConnector
    {
        public string Version { get { return Settings.CONNECTOR_VERSION; } }

        public DdsConnector(
            string serverIPv4,
            ushort serverPortTCP,
            ILogger logger)
        {
            ServerAddressIPv4 = serverIPv4 ?? throw new ArgumentNullException(nameof(serverIPv4));
            ServerPortTCP = serverPortTCP;

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            if (serverIPv4.IsValidIPv4Address())
            {
                ServerAddressIPv4 = ServerAddressIPv4.RemoveSpaces();
            }

            Logger.Info(
                $"Initializing connector version {Settings.CONNECTOR_VERSION} " +
                $"with server @{ServerAddressIPv4}:{ServerPortTCP}");
        }

        public string ServerAddressIPv4 { get; }
        public ushort ServerPortTCP { get; }
        private ILogger Logger { get; }
    }
}
