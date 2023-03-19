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

        public SingleProvider? ValueProvider { get; private set; }
        public SingleConsumer? ValueConsumer { get; private set; }

        public SingleVariable(
                    string name,
                    Periodicity periodicity,
                    SingleProvider singleProvider = null!,
                    SingleConsumer singleConsumer = null!)

            : base(name, PrimitiveType.Single, periodicity)
        {
            ValueProvider = singleProvider;
            ValueConsumer = singleConsumer;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteSingle(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                float newValue = ValueProvider(Name);

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
            return "Single";
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
