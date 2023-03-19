using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

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

                    case PacketId.VariablesRegistration:
                        ParseVariablesRegistration(ref data, ref offset);
                        break;

                    case PacketId.VariablesUpdateAtServer:
                        break;

                    case PacketId.VariablesUpdateFromServer:
                        ParseVariablesUpdateFromServer(ref data, ref offset);
                        break;

                    case PacketId.ErrorResponseFromServer:
                        string errorMessage = data.ReadString(ref offset);
                        Logger.Error($"Server Error: {errorMessage}");
                        break;

                    case PacketId.UnknownPacket:
                        Logger.Error($"Unknown message received from the server.");
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Packet parsing error: {e.Message}");
            }
        }

        private void ParseVariablesRegistration(ref byte[] data, ref int offset)
        {
            throw new NotImplementedException();
        }

        private void ParseVariablesUpdateFromServer(ref byte[] data, ref int offset)
        {
            throw new NotImplementedException();
        }
    }
}
