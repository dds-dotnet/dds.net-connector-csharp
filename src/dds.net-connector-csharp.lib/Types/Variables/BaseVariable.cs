using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables
{
    internal abstract class BaseVariable
    {
        /// <summary>
        /// Identifier for the variable.
        /// </summary>
        public ushort Id { get; private set; }
        /// <summary>
        /// Name associated with the variable.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The main type of the variable i.e., primitive or compound, etc.
        /// </summary>
        public VariableType VariableType { get; protected set; }

        /// <summary>
        /// Number of bytes that ID takes on the buffer.
        /// </summary>
        private static readonly int IdSizeOnBuffer = sizeof(short);
        /// <summary>
        /// Number of bytes that the main <c>VariableType</c> takes on the buffer.
        /// </summary>
        private static readonly int VariableTypeSizeOnBuffer = VariableType.Primitive.GetSizeOnBuffer();

        public BaseVariable(ushort id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Total size => [ID]-[Variable Type]-[Type]-[Value]
        /// </summary>
        /// <returns>Total number of bytes required on the buffer by the variable.</returns>
        public int GetSizeOnBuffer()
        {
            return
                IdSizeOnBuffer +
                VariableTypeSizeOnBuffer +
                GetSubTypeSizeOnBuffer() +
                GetValueSizeOnBuffer();
        }

        /// <summary>
        /// Write everything including ID, Type and Value on the buffer.
        /// </summary>
        /// <param name="buffer">The buffer on which to write.</param>
        /// <param name="offset">Offset in the buffer - updated after writing the data.</param>
        public void WriteOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedWord(ref offset, Id);
            buffer.WriteVariableType(ref offset, VariableType);

            WriteSubTypeOnBuffer(ref buffer, ref offset);
            WriteValueOnBuffer(ref buffer, ref offset);
        }

        /// <summary>
        /// Required size of sub-type on the buffer.
        /// </summary>
        /// <returns>Total size in bytes that is required to write the sub-type on the buffer.</returns>
        public abstract int GetSubTypeSizeOnBuffer();

        /// <summary>
        /// Required size of value on the buffer.
        /// </summary>
        /// <returns>Total number of bytes that are required to write the value on the buffer.</returns>
        public abstract int GetValueSizeOnBuffer();

        /// <summary>
        /// Writes sub-type of the data on the buffer.
        /// </summary>
        /// <param name="buffer">The buffer on which to write.</param>
        /// <param name="offset">Offset in the buffer - updated after writing the sub-type.</param>
        public abstract void WriteSubTypeOnBuffer(ref byte[] buffer, ref int offset);

        /// <summary>
        /// Writes value int the buffer.
        /// </summary>
        /// <param name="buffer">The buffer on which to write the data.</param>
        /// <param name="offset">Offset in the buffer - updated after writing the data.</param>
        public abstract void WriteValueOnBuffer(ref byte[] buffer, ref int offset);
    }
}
