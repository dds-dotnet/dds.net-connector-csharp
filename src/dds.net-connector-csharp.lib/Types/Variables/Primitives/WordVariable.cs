using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class WordVariable : BasePrimitive
    {
        public short Value { get; set; }

        public WordVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.Word;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteWord(ref offset, Value);
        }
    }
}
