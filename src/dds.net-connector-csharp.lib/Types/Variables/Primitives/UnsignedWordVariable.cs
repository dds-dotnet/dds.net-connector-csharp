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

        public UnsignedWordProvider? ValueProvider { get; private set; }
        public event UnsignedWordConsumer? ValueConsumer;

        public UnsignedWordVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedWordProvider unsignedWordProvider)

            : base(name, PrimitiveType.UnsignedWord, periodicity)
        {
            ValueProvider = unsignedWordProvider;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedWord(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                ushort newValue = ValueProvider(Name);

                if (Value != newValue)
                {
                    Value = newValue;
                    return true;
                }
            }

            return false;
        }

        protected override void ResetValue()
        {
            Value = 0;
        }
    }
}
