using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    internal class UnknownPrimitiveVariable : BasePrimitive
    {
        public UnknownPrimitiveVariable(ushort id, string name) : base(id, name)
        {
            PrimitiveType = PrimitiveType.UnknownPrimitiveType;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 0;
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
        }
    }
}
