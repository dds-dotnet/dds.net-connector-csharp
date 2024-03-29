﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>ByteVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.Byte">PrimitiveType.Byte</c> handling.
    /// </summary>
    internal class ByteVariable : BasePrimitive
    {
        public sbyte Value { get; set; }

        public ByteProvider? ValueProvider { get; private set; }
        public ByteConsumer? ValueConsumer { get; private set; }

        public ByteVariable(
                    string name,
                    Periodicity periodicity,
                    ByteProvider byteProvider = null!,
                    ByteConsumer byteConsumer = null!)

            : base(name, PrimitiveType.Byte, periodicity)
        {
            ValueProvider = byteProvider;
            ValueConsumer = byteConsumer;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 1;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteByte(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                sbyte newValue = ValueProvider(Name);

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
            return "Byte";
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
