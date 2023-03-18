using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables
{
    /// <summary>
    /// The base class for all the variables.
    /// </summary>
    internal abstract class BaseVariable
    {
        /// <summary>
        /// Identifier for the variable - assigned by the server.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Name associated with the variable.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The main type of the variable i.e., primitive or compound, etc.
        /// </summary>
        public VariableType VariableType { get; private set; }
        /// <summary>
        /// Update periodicity for the variable.
        /// </summary>
        public Periodicity Periodicity { get; private set; }

        /// <summary>
        /// Initializes the base elements.
        /// </summary>
        /// <param name="name">Name of the variable - assigned by users.</param>
        /// <param name="variableType">Main type of the variable.</param>
        /// <param name="periodicity">Periodicity of the variable.</param>
        public BaseVariable(string name, VariableType variableType, Periodicity periodicity)
        {
            Id = -1;
            Name = name;
            VariableType = variableType;
            Periodicity = periodicity;
        }

        /// <summary>
        /// Assigns ID to the variable.
        /// </summary>
        /// <param name="id">ID for the variable</param>
        /// <exception cref="ArgumentException"></exception>
        public void AssignId(int id)
        {
            if (Id != -1)
            {
                throw new Exception($"Variable {Name} has already been assigned with an ID");
            }

            if (id < 0 || id > ushort.MaxValue)
            {
                throw new ArgumentException($"Variable {Name} cannot be assigned with out-of-range ID {id}");
            }

            Id = id;
        }

        /// <summary>
        /// Resets the variable to its initial state.
        /// </summary>
        public void Reset()
        {
            Id = -1;
            ResetValue();
        }
        /// <summary>
        /// Resets the variable's value to its initial state.
        /// </summary>
        protected abstract void ResetValue();

        /// <summary>
        /// The friendly, printable name of variable's type.
        /// </summary>
        /// <returns>The type's friendly name.</returns>
        public abstract string GetPrintableTypeName();

        /// <summary>
        /// Refreshes the held value by the variable from the provider function.
        /// </summary>
        /// <returns>True = Value is changed, False = The last value is retained.</returns>
        public abstract bool RefreshValue();

        /*******************************************************************************/
        /*                                                                             */
        /* Size calculation:                                                           */
        /*     - Total bytes that the variable requires on a buffer                    */
        /*                                                                             */
        /*******************************************************************************/

        /// <summary>
        /// Number of bytes that ID takes on the buffer.
        /// </summary>
        private static readonly int IdSizeOnBuffer = sizeof(short);
        /// <summary>
        /// Number of bytes that the main <c>VariableType</c> takes on the buffer.
        /// </summary>
        private static readonly int VariableTypeSizeOnBuffer = VariableType.Primitive.GetSizeOnBuffer();

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
        /// Required size of sub-type on the buffer.
        /// </summary>
        /// <returns>Total size in bytes that is required to write the sub-type on the buffer.</returns>
        public abstract int GetSubTypeSizeOnBuffer();
        /// <summary>
        /// Required size of value on the buffer.
        /// </summary>
        /// <returns>Total number of bytes that are required to write the value on the buffer.</returns>
        public abstract int GetValueSizeOnBuffer();

        /*******************************************************************************/
        /*                                                                             */
        /* Writing on the buffer:                                                      */
        /*     - Writing along-with identifiers on the given buffer                    */
        /*                                                                             */
        /*******************************************************************************/

        /// <summary>
        /// Write everything including ID, Type and Value on the buffer.
        /// </summary>
        /// <param name="buffer">The buffer on which to write.</param>
        /// <param name="offset">Offset in the buffer - updated after writing the data.</param>
        public void WriteOnBuffer(ref byte[] buffer, ref int offset)
        {
            buffer.WriteUnsignedWord(ref offset, (ushort)Id);
            buffer.WriteVariableType(ref offset, VariableType);

            WriteSubTypeOnBuffer(ref buffer, ref offset);
            WriteValueOnBuffer(ref buffer, ref offset);
        }
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
