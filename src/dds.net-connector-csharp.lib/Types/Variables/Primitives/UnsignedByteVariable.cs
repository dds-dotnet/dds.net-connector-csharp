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
        public UnsignedByteConsumer? ValueConsumer { get; private set; }

        public UnsignedByteVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedByteProvider unsignedByteProvider = null!,
                    UnsignedByteConsumer unsignedByteConsumer = null!)

            : base(name, PrimitiveType.UnsignedByte, periodicity)
        {
            ValueProvider = unsignedByteProvider;
            ValueConsumer = unsignedByteConsumer;
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

        public override string GetPrintableTypeName()
        {
            return "Unsigned Byte";
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
