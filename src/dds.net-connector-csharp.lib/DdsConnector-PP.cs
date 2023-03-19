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
                        /*********************************************************************/
                        /* Handshake                                                         */
                        /*********************************************************************/
                        string serverName = data.ReadString(ref offset);
                        string serverVersion = data.ReadString(ref offset);

                        Logger.Info($"Server = {serverName} v{serverVersion}");
                        break;

                    case PacketId.VariablesRegistration:
                        /*********************************************************************/
                        /* Variables' Registration                                           */
                        /*********************************************************************/
                        break;

                    case PacketId.VariablesUpdateAtServer:
                        /*********************************************************************/
                        /* Variables' Update at the Server                                   */
                        /*     - Nothing required to be done at the connector end.           */
                        /*********************************************************************/
                        break;

                    case PacketId.VariablesUpdateFromServer:
                        /*********************************************************************/
                        /* Variables' Update from the Server                                 */
                        /*********************************************************************/
                        break;


                    case PacketId.ErrorResponseFromServer:
                        /*********************************************************************/
                        /* Error from the Server                                             */
                        /*********************************************************************/
                        string errorMessage = data.ReadString(ref offset);

                        Logger.Error($"Server Error: {errorMessage}");
                        break;


                    case PacketId.UnknownPacket:
                        /*********************************************************************/
                        /* Unknown Packet                                                    */
                        /*********************************************************************/
                        Logger.Error($"Unknown message received from the server.");
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
