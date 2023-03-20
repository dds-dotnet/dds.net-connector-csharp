namespace DDS.Net.Connector
{
    internal class Settings
    {
        /// <summary>
        /// Version for the connector.
        /// </summary>
        internal static string CONNECTOR_VERSION = "1.0.2";
        /// <summary>
        /// Time-slice for updating periodic variables to the server.
        /// </summary>
        internal static int BASE_TIME_SLOT_MS = 50;
    }
}
