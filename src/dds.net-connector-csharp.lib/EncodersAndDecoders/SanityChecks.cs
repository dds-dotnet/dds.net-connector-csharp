namespace DDS.Net.Connector.EncodersAndDecoders
{
    internal static class SanityChecks
    {
        /// <summary>
        /// Checks if the data buffer can be read / written with required number of bytes.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">Offset in the buffer.</param>
        /// <param name="requiredSize">Number of bytes required in the buffer.</param>
        /// <param name="readableSize">Number of bytes that are readable in the buffer.</param>
        /// <returns>The same data buffer - for chaining</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static byte[] ThrowIfNotHavingRequiredBytes(this byte[] data, ref int offset, int requiredSize, int readableSize)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    $"Offset is negative: {offset}");
            }

            int endingOffset = offset + requiredSize - 1;

            if (endingOffset >= data.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    $"Array of {data.Length} bytes requires to have " +
                    $"data with {requiredSize} bytes starting at offset {offset}");
            }

            if (endingOffset >= readableSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    $"Array having readable size of {readableSize} bytes requires to have " +
                    $"data with {requiredSize} bytes starting at offset {offset}");
            }

            return data;
        }
    }
}
