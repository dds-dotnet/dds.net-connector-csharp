using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.Primitives
{
    /// <summary>
    /// Class <c>BasePrimitive</c> is a sub-class of <c>BaseVariable</c> and
    /// represents <c cref="VariableType.Primitive">VariableType.Primitive</c> handling.
    /// </summary>
    internal abstract class BasePrimitive : BaseVariable
    {
        public PrimitiveType PrimitiveType { get; private set; }

        public BasePrimitive(string name, PrimitiveType primitiveType, Periodicity periodicity)
            : base(name, VariableType.Primitive, periodicity)
        {
            PrimitiveType = primitiveType;
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
