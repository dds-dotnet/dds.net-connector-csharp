using System.Text.RegularExpressions;

namespace DDS.Net.Connector.Helpers
{
    internal static class ExtIPAddress
    {
        private static Regex ipv4AddressPattern =
            new(@"\s*(\d{1,3})\s*\.\s*(\d{1,3})\s*\.\s*(\d{1,3})\s*\.\s*(\d{1,3})\s*");

        public static bool IsValidIPv4Address(this string ipv4Address)
        {
            return ipv4AddressPattern.IsMatch(ipv4Address);
        }
    }
}
