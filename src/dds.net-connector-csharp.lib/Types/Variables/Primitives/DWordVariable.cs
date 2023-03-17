using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class DWordVariable : BasePrimitive
    {
        public int Value { get; set; }

        public DWordVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.DWord;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteDWord(ref offset, Value);
        }
    }
}
