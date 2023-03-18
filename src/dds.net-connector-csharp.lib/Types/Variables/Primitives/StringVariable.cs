﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;
using System.Text;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>StringVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.String">PrimitiveType.String</c> handling.
    /// </summary>
    internal class StringVariable : BasePrimitive
    {
        private string _value;
        private byte[] _bytes;

        public string Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    _bytes = Encoding.Unicode.GetBytes(value);
                }
            }
        }

        public StringProvider? ValueProvider { get; private set; }
        public event StringConsumer? ValueConsumer;

        public StringVariable(
                    string name,
                    Periodicity periodicity,
                    StringProvider stringProvider)

            : base(name, PrimitiveType.String, periodicity)
        {
            _value = string.Empty;
            _bytes = Encoding.Unicode.GetBytes(_value);

            Value = string.Empty;

            ValueProvider = stringProvider;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2 + _bytes.Length;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteString(ref offset, Value);
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                string newValue = ValueProvider(Name);

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
            Value = string.Empty;
        }
    }
}
