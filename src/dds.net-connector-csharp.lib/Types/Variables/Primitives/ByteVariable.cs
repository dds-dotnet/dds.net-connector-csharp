using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>ByteVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Byte">PrimitiveType.Byte</c> handling.
    /// </summary>
    internal class ByteVariable : BasePrimitive
    {
        public sbyte Value { get; set; }

        public ByteVariable(string name, Periodicity periodicity)
            : base(name, PrimitiveType.Byte, periodicity)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteByte(ref offset, Value);
        }
    }
}
