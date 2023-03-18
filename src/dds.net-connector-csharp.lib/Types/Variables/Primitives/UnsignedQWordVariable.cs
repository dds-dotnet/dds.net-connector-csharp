﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>UnsignedQWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.UnsignedQWord">PrimitiveType.UnsignedQWord</c> handling.
    /// </summary>
    internal class UnsignedQWordVariable : BasePrimitive
    {
        public ulong Value { get; set; }

        public UnsignedQWordProvider? ValueProvider { get; private set; }
        public event UnsignedQWordConsumer? ValueConsumer;

        public UnsignedQWordVariable(
                    string name,
                    Periodicity periodicity,
                    UnsignedQWordProvider unsignedQWordProvider)

            : base(name, PrimitiveType.UnsignedQWord, periodicity)
        {
            ValueProvider = unsignedQWordProvider;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedQWord(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                ulong newValue = ValueProvider(Name);

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
