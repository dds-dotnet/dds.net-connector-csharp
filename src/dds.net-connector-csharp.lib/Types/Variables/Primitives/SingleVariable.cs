using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>SingleVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Single">PrimitiveType.Single</c> handling.
    /// </summary>
    internal class SingleVariable : BasePrimitive
    {
        public float Value { get; set; }

        public SingleVariable(string name) : base(name, PrimitiveType.Single)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteSingle(ref offset, Value);
        }
    }
}
