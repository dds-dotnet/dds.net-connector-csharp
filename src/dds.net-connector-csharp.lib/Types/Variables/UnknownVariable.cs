using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables
{
    internal class UnknownVariable : BaseVariable
    {
        public UnknownVariable(ushort id, string name) : base(id, name)
        {
            VariableType = VariableType.UnknownVariableType;
        }

        public override int GetSubTypeSizeOnBuffer()
        {
            return 0;
        }

        public override int GetValueSizeOnBuffer()
        {
            return 0;
        }

        public override void WriteSubTypeOnBuffer(ref byte[] buffer, ref int offset)
        {
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
        }
    }
}
