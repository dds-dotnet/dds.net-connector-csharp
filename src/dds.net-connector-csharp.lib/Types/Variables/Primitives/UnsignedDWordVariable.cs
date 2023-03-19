using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>UnsignedDWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.UnsignedDWord">PrimitiveType.UnsignedDWord</c> handling.
    /// </summary>
    internal class UnsignedDWordVariable : BasePrimitive
    {
        public uint Value { get; set; }

        public UnsignedDWordProvider? ValueProvider { get; private set; }
        public UnsignedDWordConsumer? ValueConsumer { get; private set; }

        public UnsignedDWordVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedDWordProvider unsignedDWordProvider = null!,
                    UnsignedDWordConsumer unsignedDWordConsumer = null!)

            : base(name, PrimitiveType.UnsignedDWord, periodicity)
        {
            ValueProvider = unsignedDWordProvider;
            ValueConsumer = unsignedDWordConsumer;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedDWord(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                uint newValue = ValueProvider(Name);

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
            return "Unsigned DWord";
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
