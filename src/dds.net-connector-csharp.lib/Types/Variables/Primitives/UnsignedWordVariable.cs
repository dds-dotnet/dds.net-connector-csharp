using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>UnsignedWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.UnsignedWord">PrimitiveType.UnsignedWord</c> handling.
    /// </summary>
    internal class UnsignedWordVariable : BasePrimitive
    {
        public ushort Value { get; set; }

        public UnsignedWordVariable(string name, Periodicity periodicity)
            : base(name, PrimitiveType.UnsignedWord, periodicity)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedWord(ref offset, Value);
        }
    }
}
