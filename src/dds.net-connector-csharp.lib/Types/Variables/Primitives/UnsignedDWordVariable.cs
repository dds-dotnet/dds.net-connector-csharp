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
        public event UnsignedDWordConsumer? ValueConsumer;

        public UnsignedDWordVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedDWordProvider unsignedDWordProvider)

            : base(name, PrimitiveType.UnsignedDWord, periodicity)
        {
            ValueProvider = unsignedDWordProvider;
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
    }
}
