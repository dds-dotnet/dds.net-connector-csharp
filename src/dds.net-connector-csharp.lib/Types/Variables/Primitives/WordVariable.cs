using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>WordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Word">PrimitiveType.Word</c> handling.
    /// </summary>
    internal class WordVariable : BasePrimitive
    {
        public short Value { get; set; }

        public WordVariable(string name, Periodicity periodicity)
            : base(name, PrimitiveType.Word, periodicity)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteWord(ref offset, Value);
        }
    }
}
