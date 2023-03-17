using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class UnsignedWordVariable : BasePrimitive
    {
        public ushort Value { get; set; }

        public UnsignedWordVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.UnsignedWord;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedWord(ref offset, Value);
        }
    }
}
