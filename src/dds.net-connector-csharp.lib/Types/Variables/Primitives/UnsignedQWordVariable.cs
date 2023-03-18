using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>UnsignedQWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.UnsignedQWord">PrimitiveType.UnsignedQWord</c> handling.
    /// </summary>
    internal class UnsignedQWordVariable : BasePrimitive
    {
        public ulong Value { get; set; }

        public UnsignedQWordVariable(string name) : base(name, PrimitiveType.UnsignedQWord)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedQWord(ref offset, Value);
        }
    }
}
