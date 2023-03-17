using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class UnsignedDWordVariable : BasePrimitive
    {
        public uint Value { get; set; }

        public UnsignedDWordVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.UnsignedDWord;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedDWord(ref offset, Value);
        }
    }
}
