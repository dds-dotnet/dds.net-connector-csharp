namespace DDS.Net.Connector
{
    internal class Settings
    {
        /// <summary>
        /// Version for the connector.
        /// </summary>
        internal static string CONNECTOR_VERSION = "1.0.1";
        /// <summary>
        /// Time-slice for updating periodic variables to the server.
        /// </summary>
        internal static int BASE_TIME_SLOT_MS = 50;
        /// <summary>
        /// Maximum size of packet being received.
        /// </summary>
        internal static int MAX_RECEIVE_PACKET_SIZE = 4096;
    }
}
