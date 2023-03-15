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
    /// <summary>
    /// Class <c>DdsConnector</c> provides interface for connecting with
    /// DDS.Net Server.
    /// </summary>
    public class DdsConnector
    {
        /// <summary>
        /// Version number of the connector.
        /// </summary>
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

        /// <summary>
        /// DDS.Net Server's IPv4 address.
        /// </summary>
        public string ServerAddressIPv4 { get; }
        /// <summary>
        /// DDS.Net Server's TCP port.
        /// </summary>
        public ushort ServerPortTCP { get; }

        private ILogger Logger { get; }

        /***********************************************************************************/
        /*                                                                                 */
        /* Starting / stopping connection wiht the server                                  */
        /*                                                                                 */
        /***********************************************************************************/

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

        /***********************************************************************************/
        /*                                                                                 */
        /* Registering data providers and consumers                                        */
        /*                                                                                 */
        /***********************************************************************************/

        public void RegisterStringProvider(string variableName, StringProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterBooleanProvider(string variableName, BooleanProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterByteProvider(string variableName, ByteProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterWordProvider(string variableName, WordProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterDWordProvider(string variableName, DWordProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterQWordProvider(string variableName, QWordProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedByteProvider(string variableName, UnsignedByteProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedWordProvider(string variableName, UnsignedWordProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedDWordProvider(string variableName, UnsignedDWordProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedQWordProvider(string variableName, UnsignedQWordProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterSingleProvider(string variableName, SingleProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterDoubleProvider(string variableName, DoubleProvider provider, Periodicity periodicity)
        {

        }
        public void RegisterRawBytesProvider(string variableName, RawBytesProvider provider, Periodicity periodicity)
        {

        }

        public void UnregisterProvider(string variableName)
        {

        }

        public void RegisterStringConsumer(string variableName, StringConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterBooleanConsumer(string variableName, BooleanConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterByteConsumer(string variableName, ByteConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterWordConsumer(string variableName, WordConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterDWordConsumer(string variableName, DWordConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterQWordConsumer(string variableName, QWordConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedByteConsumer(string variableName, UnsignedByteConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedWordConsumer(string variableName, UnsignedWordConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedDWordConsumer(string variableName, UnsignedDWordConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterUnsignedQWordConsumer(string variableName, UnsignedQWordConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterSingleConsumer(string variableName, SingleConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterDoubleConsumer(string variableName, DoubleConsumer consumer, Periodicity periodicity)
        {

        }
        public void RegisterRawBytesConsumer(string variableName, RawBytesConsumer consumer, Periodicity periodicity)
        {

        }

        public void RegisterConsumer(string variableName)
        {

        }
    }
}
