using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>DWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.DWord">PrimitiveType.DWord</c> handling.
    /// </summary>
    internal class DWordVariable : BasePrimitive
    {
        public int Value { get; set; }

        public DWordProvider? ValueProvider { get; private set; }
        public DWordConsumer? ValueConsumer { get; private set; }

        public DWordVariable(
                    string name,
                    Periodicity periodicity,
                    DWordProvider dWordProvider = null!,
                    DWordConsumer dWordConsumer = null!)

            : base(name, PrimitiveType.DWord, periodicity)
        {
            ValueProvider = dWordProvider;
            ValueConsumer = dWordConsumer;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteDWord(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                int newValue = ValueProvider(Name);

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
            return "DWord";
        }
    }
}
