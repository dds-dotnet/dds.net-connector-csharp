using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class BooleanVariable : BasePrimitive
    {
        public bool Value { get; set; }

        public BooleanVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.Boolean;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteBoolean(ref offset, Value);
        }
    }
}
