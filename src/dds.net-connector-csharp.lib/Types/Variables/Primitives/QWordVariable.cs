using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>QWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.QWord">PrimitiveType.QWord</c> handling.
    /// </summary>
    internal class QWordVariable : BasePrimitive
    {
        public long Value { get; set; }

        public QWordProvider? ValueProvider { get; private set; }
        public event QWordConsumer? ValueConsumer;

        public QWordVariable(
                    string name,
                    Periodicity periodicity,
                    QWordProvider qWordProvider)

            : base(name, PrimitiveType.QWord, periodicity)
        {
            ValueProvider = qWordProvider;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteQWord(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                long newValue = ValueProvider(Name);

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
