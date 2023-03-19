using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces;
using DDS.Net.Connector.Interfaces.NetworkClient;
using DDS.Net.Connector.Types.Enumerations;
using DDS.Net.Connector.Types.Variables;
using DDS.Net.Connector.Types.Variables.Primitives;
using DDS.Net.Connector.Types.Variables.RawBytes;

namespace DDS.Net.Connector
{
    /// <summary>
    /// Class <c>DdsConnector</c> provides interface for connecting with
    /// DDS.Net Server.
    /// </summary>
    public partial class DdsConnector
    {
        #region Initialization
        /// <summary>
        /// Version number of the connector.
        /// </summary>
        public static string LibraryVersion { get { return Settings.CONNECTOR_VERSION; } }
        /// <summary>
        /// Application's name.
        /// </summary>
        public string ApplicationName { get; }

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
        /// <summary>
        /// Interface with the network.
        /// </summary>
        private IThreadedNetworkClient NetworkClient { get; }
        /// <summary>
        /// Queue of data packets from the server.
        /// </summary>
        private ISyncQueueReader<PacketFromServer> DataFromServer { get; }
        /// <summary>
        /// Queue of data packets to the server.
        /// </summary>
        private ISyncQueueWriter<PacketToServer> DataToServer { get; }

        private EasyThread<DdsConnector> dataReceiverThread;
        private EasyThread<DdsConnector> periodicUpdateThread;

        /// <summary>
        /// Initializes class instance for <c>DdsConnector</c> to communicate with DDS.Net Server.
        /// </summary>
        /// <param name="applicationName">User application's name.</param>
        /// <param name="serverIPv4">Target server's IPv4 address.</param>
        /// <param name="serverPortTCP">Target server's TCP port number.</param>
        /// <param name="logger">Instance of <c cref="ILogger">ILogger.</c></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DdsConnector(
            string applicationName,
            string serverIPv4,
            ushort serverPortTCP,
            ILogger logger)
        {
            ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
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

            try
            {
                NetworkClient = new NetworkClient();
                DataFromServer = NetworkClient.GetDataQueueFromServer();
                DataToServer = NetworkClient.GetDataQueueToServer();
            }
            catch (Exception ex)
            {
                string errorMessage = $"Cannot initialize network client - {ex.Message}";

                Logger.Error(errorMessage);

                throw new Exception(errorMessage);
            }

            dataReceiverThread = new(DataReceptionWorker, this);
            periodicUpdateThread = new(PeriodicUpdateWorker, this, Settings.BASE_TIME_SLOT_MS);
        }
        #endregion
        /***********************************************************************************/
        /*                                                                                 */
        /* Starting / stopping connection with the server                                  */
        /*                                                                                 */
        /***********************************************************************************/
        #region Start / Stop - Background Work
        /// <summary>
        /// Starting the connection activity.
        /// </summary>
        public void Start()
        {
            NetworkClient.Connect(ServerAddressIPv4, ServerPortTCP);

            dataReceiverThread.Start();
            periodicUpdateThread.Start();

            byte[] handshake = new byte[100];
            int offset = 0;

            handshake.WritePacketId(ref offset, PacketId.HandShake);
            handshake.WriteString(ref offset, ApplicationName);
            handshake.WriteString(ref offset, LibraryVersion);

            DataToServer.Enqueue(new(handshake, offset));
        }

        /// <summary>
        /// Stopping the connection activity.
        /// </summary>
        public void Stop()
        {
            NetworkClient.Disconnect();

            periodicUpdateThread.Stop();
            dataReceiverThread.Stop();

            lock (variablesMutex)
            {
                foreach (KeyValuePair<string, BaseVariable> v in uploadVariables)
                {
                    v.Value.Reset();
                    uploadVariablesToBeRegistered.Add(v.Key, v.Value);
                }

                foreach (KeyValuePair<string, BaseVariable> v in downloadVariables)
                {
                    v.Value.Reset();
                    downloadVariablesToBeRegistered.Add(v.Key, v.Value);
                }

                uploadVariables.Clear();
                downloadVariables.Clear();
            }

            GC.Collect();
        }

        private static bool DataReceptionWorker(DdsConnector connector)
        {
            bool doneAnything = false;

            if (connector.DataFromServer.CanDequeue())
            {
                doneAnything = true;

                PacketFromServer packet = connector.DataFromServer.Dequeue();

                connector.ParsePacket(packet.Data);
            }

            return doneAnything;
        }

        private int iterationCounter = 0;

        private static void PeriodicUpdateWorker(DdsConnector connector)
        {
            connector.iterationCounter++;

            connector.DoPeriodicUpdate(Periodicity.Highest);

            if (connector.iterationCounter % 2 == 0) connector.DoPeriodicUpdate(Periodicity.High);
            if (connector.iterationCounter % 4 == 0) connector.DoPeriodicUpdate(Periodicity.Normal);
            if (connector.iterationCounter % 8 == 0) connector.DoPeriodicUpdate(Periodicity.Low);

            if (connector.iterationCounter % 16 == 0)
            {
                connector.DoPeriodicUpdate(Periodicity.Lowest);
                connector.iterationCounter = 0;
            }
        }
        #endregion
        /***********************************************************************************/
        /*                                                                                 */
        /* Registering data providers and consumers                                        */
        /*                                                                                 */
        /***********************************************************************************/
        private Mutex variablesMutex = new();

        private Dictionary<string, BaseVariable> uploadVariables = new();
        private Dictionary<string, BaseVariable> downloadVariables = new();

        private Dictionary<string, BaseVariable> uploadVariablesToBeRegistered = new();
        private Dictionary<string, BaseVariable> downloadVariablesToBeRegistered = new();

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
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new StringVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Boolean" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterBooleanProvider(string variableName, BooleanProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new BooleanVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Byte" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterByteProvider(string variableName, ByteProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new ByteVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterWordProvider(string variableName, WordProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new WordVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Double Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDWordProvider(string variableName, DWordProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new DWordVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Signed Quad Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterQWordProvider(string variableName, QWordProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new QWordVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Byte" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedByteProvider(string variableName, UnsignedByteProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new UnsignedByteVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedWordProvider(string variableName, UnsignedWordProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new UnsignedWordVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Double Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedDWordProvider(string variableName, UnsignedDWordProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new UnsignedDWordVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Unsigned Quad Word" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedQWordProvider(string variableName, UnsignedQWordProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new UnsignedQWordVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Single - 4 byte floating-point" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterSingleProvider(string variableName, SingleProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new SingleVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "Double - 8 byte floating-point" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDoubleProvider(string variableName, DoubleProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new DoubleVariable(variableName, periodicity, provider));
                }
            }
        }
        /// <summary>
        /// Registers a provider delegate for providing "bytes array" to the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterRawBytesProvider(string variableName, RawBytesProvider provider, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName) ||
                    uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision to the server.");
                }
                else
                {
                    uploadVariablesToBeRegistered.Add(variableName, new RawBytesVariable(variableName, periodicity, provider));
                }
            }
        }

        /// <summary>
        /// Unregisters providing named variable to the server.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        public void UnregisterProvider(string variableName)
        {
            lock (variablesMutex)
            {
                if (uploadVariables.ContainsKey(variableName))
                {
                    uploadVariables.Remove(variableName);
                }

                if (uploadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    uploadVariablesToBeRegistered.Remove(variableName);
                }
            }
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
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new StringVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Boolean" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterBooleanConsumer(string variableName, BooleanConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new BooleanVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Byte" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterByteConsumer(string variableName, ByteConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new ByteVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterWordConsumer(string variableName, WordConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new WordVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Double Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterDWordConsumer(string variableName, DWordConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new DWordVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Signed Quad Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterQWordConsumer(string variableName, QWordConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new QWordVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Byte" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedByteConsumer(string variableName, UnsignedByteConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new UnsignedByteVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedWordConsumer(string variableName, UnsignedWordConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new UnsignedWordVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Double Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedDWordConsumer(string variableName, UnsignedDWordConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new UnsignedDWordVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "Unsigned Quad Word" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterUnsignedQWordConsumer(string variableName, UnsignedQWordConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new UnsignedQWordVariable(variableName, periodicity, null!, consumer));
                }
            }
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
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new SingleVariable(variableName, periodicity, null!, consumer));
                }
            }
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
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new DoubleVariable(variableName, periodicity, null!, consumer));
                }
            }
        }
        /// <summary>
        /// Registers a consumer delegate for receiving "bytes array" from the server at given periodicity.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        /// <param name="provider">The delegate.</param>
        /// <param name="periodicity">Periodicity.</param>
        public void RegisterRawBytesConsumer(string variableName, RawBytesConsumer consumer, Periodicity periodicity)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName) ||
                    downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    throw new Exception(
                        $"The variable named {variableName} " +
                        $"has already been registered for provision from the server.");
                }
                else
                {
                    downloadVariablesToBeRegistered.Add(
                        variableName, new RawBytesVariable(variableName, periodicity, null!, consumer));
                }
            }
        }

        /// <summary>
        /// Unregisters getting named variable from the server.
        /// </summary>
        /// <param name="variableName">Variable's name.</param>
        public void UnregisterConsumer(string variableName)
        {
            lock (variablesMutex)
            {
                if (downloadVariables.ContainsKey(variableName))
                {
                    downloadVariables.Remove(variableName);
                }

                if (downloadVariablesToBeRegistered.ContainsKey(variableName))
                {
                    downloadVariablesToBeRegistered.Remove(variableName);
                }
            }
        }
        #endregion
    }
}
