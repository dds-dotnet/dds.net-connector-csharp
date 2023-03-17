using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.EncodersAndDecoders
{
    internal static class EncDecPrimitiveType
    {
        /// <summary>
        /// Reads <c cref="PrimitiveType">PrimitiveType</c> from the data buffer
        /// and updates the offset past the <c cref="PrimitiveType">PrimitiveType</c>.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">Offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <returns><c cref="PrimitiveType">PrimitiveType</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PrimitiveType ReadPrimitiveType(this byte[] data, ref int offset)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 1);

            int v = data[offset++];

            if (v >= 0 && v < (int)PrimitiveType.UnknownPrimitiveType)
            {
                return (PrimitiveType)v;
            }

            return PrimitiveType.UnknownPrimitiveType;
        }

        /// <summary>
        /// Writes <c cref="PrimitiveType">PrimitiveType</c> to the given data buffer.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">Offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <param name="value">The value to be written on the buffer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void WritePrimitiveType(this byte[] data, ref int offset, PrimitiveType value)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 1);

            data[offset++] = (byte)value;
        }

        /// <summary>
        /// Size in bytes <c cref="PrimitiveType">PrimitiveType</c> requires on a buffer
        /// </summary>
        /// <param name="_"></param>
        /// <returns>Number of bytes required on the buffer</returns>
        public static int GetSizeOnBuffer(this PrimitiveType _)
        {
            return 1;
        }
    }
}
