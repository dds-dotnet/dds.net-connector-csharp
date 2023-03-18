using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>UnsignedByteVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.UnsignedByte">PrimitiveType.UnsignedByte</c> handling.
    /// </summary>
    internal class UnsignedByteVariable : BasePrimitive
    {
        public byte Value { get; set; }

        public UnsignedByteProvider? ValueProvider { get; private set; }
        public event UnsignedByteConsumer? ValueConsumer;

        public UnsignedByteVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedByteProvider unsignedByteProvider)

            : base(name, PrimitiveType.UnsignedByte, periodicity)
        {
            ValueProvider = unsignedByteProvider;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedByte(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                byte newValue = ValueProvider(Name);

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
