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
        public UnsignedWordConsumer? ValueConsumer { get; private set; }

        public UnsignedWordVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedWordProvider unsignedWordProvider = null!,
                    UnsignedWordConsumer unsignedWordConsumer = null!)

            : base(name, PrimitiveType.UnsignedWord, periodicity)
        {
            ValueProvider = unsignedWordProvider;
            ValueConsumer = unsignedWordConsumer;
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

        public override string GetPrintableTypeName()
        {
            return "Unsigned Word";
        }

        public override void InvokeValueAwaiter()
        {
            if (ValueConsumer != null)
            {
                ValueConsumer(Name, Value);
            }
        }
    }
}
