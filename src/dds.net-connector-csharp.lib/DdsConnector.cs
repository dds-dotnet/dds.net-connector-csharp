using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces;
using DDS.Net.Connector.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDS.Net.Connector
{
    public class DdsConnector
    {
        public static string Version { get { return Settings.CONNECTOR_VERSION; } }

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
                $"with target server @{ServerAddressIPv4}:{ServerPortTCP}");
        }

        public string ServerAddressIPv4 { get; }
        public ushort ServerPortTCP { get; }
        private ILogger Logger { get; }

        /// <summary>
        /// Starting the connection activity.
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// Stopping the connection activity.
        /// </summary>
        public void Stop()
        {

        }

        public void RegisterStringProvider(string variableName, StringProvider provider, Periodicity periodicity)
        {

        }
    }
}
