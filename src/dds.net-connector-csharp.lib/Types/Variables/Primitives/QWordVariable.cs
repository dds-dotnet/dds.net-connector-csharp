﻿using DDS.Net.Connector.EncodersAndDecoders;
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

        public QWordVariable(string name, Periodicity periodicity)
            : base(name, PrimitiveType.QWord, periodicity)
        {
        }

        public override int GetValueSizeOnBuffer()
        {
            return 8;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteQWord(ref offset, Value);
        }
    }
}
