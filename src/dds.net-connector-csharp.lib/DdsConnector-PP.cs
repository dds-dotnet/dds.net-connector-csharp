using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types;

namespace DDS.Net.Connector
{
    public partial class DdsConnector
    {
        private void ParsePacket(byte[] data)
        {
            int offset = 0;

            try
            {
                PacketId packetId = data.ReadPacketId(ref offset);

                switch (packetId)
                {
                    case PacketId.HandShake:
                        string serverName = data.ReadString(ref offset);
                        string serverVersion = data.ReadString(ref offset);

                        Logger.Info($"Server = {serverName} v{serverVersion}");
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Packet parsing error: {e.Message}");
            }
        }
    }
}
