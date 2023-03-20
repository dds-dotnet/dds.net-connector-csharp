namespace DDS.Net.Connector.Interfaces.NetworkClient
{
    internal class PacketFromServer
    {
        public byte[] Data { get; }
        public int Size { get; }

        public PacketFromServer(byte[] data, int size)
        {
            Data = data;
            Size = size;
        }
    }
}
