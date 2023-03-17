using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.EncodersAndDecoders
{
    internal static class EncDecPeriodicity
    {
        /// <summary>
        /// Reads <c cref="Periodicity">Periodicity</c> from the data buffer
        /// and updates the offset past the <c cref="Periodicity">Periodicity</c>.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">Offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <returns><c cref="Periodicity">Periodicity</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static Periodicity ReadPeriodicity(this byte[] data, ref int offset)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 1);

            int v = data[offset++];

            if (v >= 0 && v <= (int)Periodicity.Lowest)
            {
                return (Periodicity)v;
            }

            return Periodicity.OnChange;
        }

        /// <summary>
        /// Writes <c cref="Periodicity">Periodicity</c> to the given data buffer.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">Offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <param name="value">Value to be written to the buffer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static void WritePeriodicity(this byte[] data, ref int offset, Periodicity value)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 1);

            data[offset++] = (byte)value;
        }

        /// <summary>
        /// Size in bytes that <c cref="Periodicity">Periodicity</c> requires on a buffer.
        /// </summary>
        /// <param name="_"></param>
        /// <returns>Number of bytes required on the buffer</returns>
        internal static int GetSizeOnBuffer(this Periodicity _)
        {
            return 1;
        }
    }
}
