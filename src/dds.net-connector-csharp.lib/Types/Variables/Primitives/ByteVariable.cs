using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class ByteVariable : BasePrimitive
    {
        public sbyte Value { get; set; }

        public ByteVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.Byte;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteByte(ref offset, Value);
        }
    }
}
