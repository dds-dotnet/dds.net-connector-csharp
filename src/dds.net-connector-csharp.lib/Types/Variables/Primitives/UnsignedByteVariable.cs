using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class UnsignedByteVariable : BasePrimitive
    {
        public byte Value { get; set; }

        public UnsignedByteVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.UnsignedByte;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedByte(ref offset, Value);
        }
    }
}
