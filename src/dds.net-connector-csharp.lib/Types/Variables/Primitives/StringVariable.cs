﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;
using System.Text;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
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

        public StringVariable(ushort id, string name) : base(id, name)
        {
            _value = string.Empty;
            _bytes = Encoding.Unicode.GetBytes(_value);

            Value = string.Empty;
            PrimitiveType = PrimitiveType.String;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 2 + _bytes.Length;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteString(ref offset, Value);
        }
    }
}
