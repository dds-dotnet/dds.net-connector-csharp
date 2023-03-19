using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>WordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Word">PrimitiveType.Word</c> handling.
    /// </summary>
    internal class WordVariable : BasePrimitive
    {
        public short Value { get; set; }

        public WordProvider? ValueProvider { get; private set; }
        public WordConsumer? ValueConsumer { get; private set; }

        public WordVariable(
                    string name,
                    Periodicity periodicity,
                    WordProvider wordProvider = null!,
                    WordConsumer wordConsumer = null!)

            : base(name, PrimitiveType.Word, periodicity)
        {
            ValueProvider = wordProvider;
            ValueConsumer = wordConsumer;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteWord(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                short newValue = ValueProvider(Name);

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
            return "Word";
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
