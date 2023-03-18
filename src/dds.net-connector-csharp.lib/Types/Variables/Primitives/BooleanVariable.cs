using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>BooleanVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Boolean">PrimitiveType.Boolean</c> handling.
    /// </summary>
    internal class BooleanVariable : BasePrimitive
    {
        public bool Value { get; set; }

        public BooleanVariable(string name) : base(name, PrimitiveType.Boolean)
        {
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
