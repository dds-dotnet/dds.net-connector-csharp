﻿using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal abstract class BasePrimitive : BaseVariable
    {
        public PrimitiveType PrimitiveType { get; protected set; } = PrimitiveType.UnknownPrimitiveType;

        public BasePrimitive(ushort id, string name) : base(id, name)
        {
            VariableType = VariableType.Primitive;
        }

        public override int GetSubTypeSizeOnBuffer()
        {
            return PrimitiveType.GetSizeOnBuffer();
        }

        public override void WriteSubTypeOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WritePrimitiveType(ref offset, PrimitiveType);
        }
    }
}
