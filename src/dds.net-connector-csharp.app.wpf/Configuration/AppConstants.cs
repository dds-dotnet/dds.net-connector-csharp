using System;

namespace DDS.Net.Connector.WpfApp.Configuration
{
    internal static class AppConstants
    {
        static private readonly DateTime TIMESTAMP = DateTime.Now;
        static private readonly string TIMESTAMP_TEXT =
            $"{TIMESTAMP.Year}_{TIMESTAMP.Month,02:00}_{TIMESTAMP.Day,02:00}_" +
            $"{TIMESTAMP.Hour,02:00}_{TIMESTAMP.Minute,02:00}_{TIMESTAMP.Second,02:00}";

        static public readonly string LOG_FILENAME = $"Log/log-wpf-{TIMESTAMP_TEXT}.txt";
        static public readonly string SERVER_CONFIG_FILENAME = "Configuration/server-conf.ini";
    }
}
