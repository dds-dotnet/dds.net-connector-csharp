﻿using DDS.Net.Connector.Types;

namespace DDS.Net.Connector.EncodersAndDecoders
{
    internal static class EncDecPacketId
    {
        /// <summary>
        /// Reads <c cref="PacketId">PacketId</c> from the data buffer and
        /// updates the offset past the <c cref="PacketId">PacketId</c>.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <returns><c cref="PacketId">PacketId</c></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PacketId ReadPacketId(this byte[] data, ref int offset)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 2);

            int v = data[offset++];
            v = (v << 8) | data[offset++];

            if (v >= 0 && v < (int)PacketId.UnknownPacket)
            {
                return (PacketId)v;
            }

            return PacketId.UnknownPacket;
        }

        /// <summary>
        /// Writes <c cref="PacketId">PacketId</c> to the given data buffer.
        /// </summary>
        /// <param name="data">The buffer containing data.</param>
        /// <param name="offset">Offset in the data buffer - updated afterwards to point
        /// to the next element in the buffer.</param>
        /// <param name="value">Value to be written to the buffer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static void WritePacketId(this byte[] data, ref int offset, PacketId value)
        {
            data.ThrowIfNotHavingRequiredBytes(ref offset, 2);

            int v = (int)value;

            data[offset++] = (byte)((v >> 8) & 0x0ff);
            data[offset++] = (byte)((v >> 0) & 0x0ff);
        }

        /// <summary>
        /// Size in bytes <c cref="PacketId">PacketId</c> requires on a buffer.
        /// </summary>
        /// <param name="_"></param>
        /// <returns>Number of bytes required on the buffer</returns>
        internal static int GetSizeOnBuffer(this PacketId _)
        {
            return 2;
        }
    }
}
