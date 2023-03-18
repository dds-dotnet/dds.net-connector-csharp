using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>DoubleVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Double">PrimitiveType.Double</c> handling.
    /// </summary>
    internal class DoubleVariable : BasePrimitive
    {
        public double Value { get; set; }

        public DoubleProvider? ValueProvider { get; private set; }
        public event DoubleConsumer? ValueConsumer;

        public DoubleVariable(
                    string name,
                    Periodicity periodicity,
                    DoubleProvider doubleProvider)

            : base(name, PrimitiveType.Double, periodicity)
        {
            ValueProvider = doubleProvider;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteDouble(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                double newValue = ValueProvider(Name);

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
