using DDS.Net.Connector.Types;

namespace DDS.Net.Connector.EncodersAndDecoders
{
    internal static class EncDecVariableType
    {
        /// <summary>
        /// Reads <c cref="VariableType">VariableType</c> from the data buffer
        /// and updates the offset past the <c cref="VariableType">VariableType</c>.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <returns><c cref="VariableType">VariableType</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static VariableType ReadVariableType(this byte[] data, ref int offset)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 1);

            int v = data[offset++];

            if (v >= 0 && v < (int)VariableType.UnknownVariableType)
            {
                return (VariableType)v;
            }

            return VariableType.UnknownVariableType;
        }

        /// <summary>
        /// Writes <c cref="VariableType">VariableType</c> to the given data buffer.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">Offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <param name="value">The value to be written to the buffer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void WriteVariableType(this byte[] data, ref int offset, VariableType value)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 1);

            data[offset++] = (byte)value;
        }

        /// <summary>
        /// Size in bytes that <c cref="VariableType">VariableType</c> requires on a buffer.
        /// </summary>
        /// <param name="_"></param>
        /// <returns>Number of bytes required on the buffer</returns>
        public static int GetSizeOnBuffer(this VariableType _)
        {
            return 1;
        }
    }
}
