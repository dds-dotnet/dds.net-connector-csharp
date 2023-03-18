using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>BooleanVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Boolean">PrimitiveType.Boolean</c> handling.
    /// </summary>
    internal class BooleanVariable : BasePrimitive
    {
        public bool Value { get; set; }

        public BooleanProvider? ValueProvider { get; private set; }
        public BooleanConsumer? ValueConsumer { get; private set; }

        public BooleanVariable(
                    string name,
                    Periodicity periodicity,
                    BooleanProvider booleanProvider = null!,
                    BooleanConsumer booleanConsumer = null!)

            : base(name, PrimitiveType.Boolean, periodicity)
        {
            ValueProvider = booleanProvider;
            ValueConsumer = booleanConsumer;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteBoolean(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                bool newValue = ValueProvider(Name);

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
            Value = false;
        }

        public override string GetPrintableTypeName()
        {
            return "Boolean";
        }
    }
}
