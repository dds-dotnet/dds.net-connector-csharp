﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class SingleVariable : BasePrimitive
    {
        public float Value { get; set; }

        public SingleVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.Single;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 4;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteSingle(ref offset, Value);
        }
    }
}
