namespace DDS.Net.Connector.Interfaces.NetworkClient
{
    internal class PacketToServer
    {
        public byte[] Data { get; }
        public int TotalBytesToBeSent { get; }

        public PacketToServer(byte[] data, int size)
        {
            Data = data;
            TotalBytesToBeSent = size;
        }
    }
}
