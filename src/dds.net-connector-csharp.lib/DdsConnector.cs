using DDS.Net.Connector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDS.Net.Connector
{
    public class DdsConnector
    {
        public DdsConnector(string serverIPv4, ushort serverPortTCP)
        {
            ServerAddressIPv4 = serverIPv4 ?? throw new ArgumentNullException(nameof(serverIPv4));
            ServerPortTCP = serverPortTCP;

            if (serverIPv4.IsValidIPv4Address())
            {
                ServerAddressIPv4 = ServerAddressIPv4.RemoveSpaces();
            }
        }

        public string ServerAddressIPv4 { get; }
        public ushort ServerPortTCP { get; }
    }
}
