﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>DWordVariable</c> is a sub-class of <c>BasePrimitive</c> and
    /// represents <c cref="PrimitiveType.DWord">PrimitiveType.DWord</c> handling.
    /// </summary>
    internal class DWordVariable : BasePrimitive
    {
        public int Value { get; set; }

        public DWordVariable(string name) : base(name, PrimitiveType.DWord)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteDWord(ref offset, Value);
        }
    }
}
