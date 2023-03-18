using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>DoubleVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Double">PrimitiveType.Double</c> handling.
    /// </summary>
    internal class DoubleVariable : BasePrimitive
    {
        public double Value { get; set; }

        public DoubleVariable(string name) : base(name, PrimitiveType.Double)
        {
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
