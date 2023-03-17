using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class QWordVariable : BasePrimitive
    {
        public long Value { get; set; }

        public QWordVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.QWord;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteQWord(ref offset, Value);
        }
    }
}
