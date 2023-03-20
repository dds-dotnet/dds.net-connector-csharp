namespace DDS.Net.Connector.Interfaces.NetworkClient
{
    internal class PacketFromServer
    {
        public byte[] Data { get; }

        public PacketFromServer(byte[] data)
        {
            Data = data;
        }
    }
}
