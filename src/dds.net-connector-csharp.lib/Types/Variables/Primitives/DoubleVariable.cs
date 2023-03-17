using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class DoubleVariable : BasePrimitive
    {
        public double Value { get; set; }

        public DoubleVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.Double;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteDouble(ref offset, Value);
        }
    }
}
