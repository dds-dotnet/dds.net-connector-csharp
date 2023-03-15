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
        /// <summary>
        /// DDS.Net Server's IPv4 address.
        /// </summary>
        public string ServerAddressIPv4 { get; }
        /// <summary>
        /// DDS.Net Server's TCP port.
        /// </summary>
        public ushort ServerPortTCP { get; }
        /// <summary>
        /// Logging interface.
        /// </summary>
        private ILogger Logger { get; }

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
        #region Providers
        //- 
        //- Providers
        //- 

        /// <summary>
        /// Registers a provider delegate for providing "String" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterStringProvider(string variableName, StringProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Boolean" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterBooleanProvider(string variableName, BooleanProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Byte" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterByteProvider(string variableName, ByteProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterWordProvider(string variableName, WordProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Double Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDWordProvider(string variableName, DWordProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Quad Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterQWordProvider(string variableName, QWordProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Byte" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedByteProvider(string variableName, UnsignedByteProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedWordProvider(string variableName, UnsignedWordProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Double Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedDWordProvider(string variableName, UnsignedDWordProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Quad Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedQWordProvider(string variableName, UnsignedQWordProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Single - 4 byte floating-point" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterSingleProvider(string variableName, SingleProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "Double - 8 byte floating-point" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDoubleProvider(string variableName, DoubleProvider provider, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a provider delegate for providing "bytes array" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterRawBytesProvider(string variableName, RawBytesProvider provider, Periodicity periodicity)
        {

        }

        /// <summary>
        /// Unregisters providing named variable to the server.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        public void UnregisterProvider(string variableName)
        {

        }
        #endregion
        #region Consumers
        //- 
        //- Consumers
        //- 

        /// <summary>
        /// Registers a consumer delegate for receiving "String" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterStringConsumer(string variableName, StringConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Boolean" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterBooleanConsumer(string variableName, BooleanConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Byte" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterByteConsumer(string variableName, ByteConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterWordConsumer(string variableName, WordConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Double Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDWordConsumer(string variableName, DWordConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Quad Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterQWordConsumer(string variableName, QWordConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Byte" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedByteConsumer(string variableName, UnsignedByteConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedWordConsumer(string variableName, UnsignedWordConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Double Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedDWordConsumer(string variableName, UnsignedDWordConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Quad Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedQWordConsumer(string variableName, UnsignedQWordConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Single - 4 byte floating-point number"
        /// from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterSingleConsumer(string variableName, SingleConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Double - 8 byte floating-point number"
        /// from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDoubleConsumer(string variableName, DoubleConsumer consumer, Periodicity periodicity)
        {

        }
        /// <summary>
        /// Registers a consumer delegate for receiving "bytes array" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterRawBytesConsumer(string variableName, RawBytesConsumer consumer, Periodicity periodicity)
        {

        }

        /// <summary>
        /// Unregisters getting named variable from the server.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        public void UnregisterConsumer(string variableName)
        {

        }
        #endregion
    }
}
