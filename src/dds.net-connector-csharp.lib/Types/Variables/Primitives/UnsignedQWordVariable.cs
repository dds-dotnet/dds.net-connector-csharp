using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class UnsignedQWordVariable : BasePrimitive
    {
        public ulong Value { get; set; }

        public UnsignedQWordVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.UnsignedQWord;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedQWord(ref offset, Value);
        }
    }
}
