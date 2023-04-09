using DDS.Net.Connector.Interfaces.NetworkClient;

namespace DDS.Net.Connector.EncodersAndDecoders
{
    internal class PacketPreprocessor
    {
        private Mutex mutex = new();
        private byte[] previousData = null!;
        private int previousDataStartIndex = 0;
        private int previousNextWriteIndex = 0;

        internal void AddData(PacketFromServer packet)
        {
            lock (mutex)
            {
                //- 
                //- Buffer: [ - - - - - - - - - - - - - - - - - - - - - - - - - - - ]
                //-                   Data: - - - - - -
                //-           Start Index _/            \_ Next Write Index
                //- 

                byte[] buffer = null!;
                int bufferStartIndex = 0;
                int bufferNextWriteIndex = 0;

                //- 
                //- No previous data available
                //- 
                if (previousData == null)
                {
                    buffer = new byte[packet.Data.Length];
                    bufferStartIndex = 0;
                    bufferNextWriteIndex = 0;

                    previousData = buffer;
                    previousDataStartIndex = bufferStartIndex;
                    previousNextWriteIndex = bufferNextWriteIndex;
                }
                //- 
                //- We have some previous data
                //- 
                else
                {
                    buffer = previousData;
                    bufferStartIndex = previousDataStartIndex;
                    bufferNextWriteIndex = previousNextWriteIndex;
                }

                //- 
                //- Compacting the buffer
                //- 
                if (bufferStartIndex > 0)
                {
                    for (int i = 0; i < (bufferNextWriteIndex - bufferStartIndex); i++)
                    {
                        buffer[i] = buffer[bufferStartIndex + i];
                    }

                    bufferNextWriteIndex -= bufferStartIndex;
                    bufferStartIndex = 0;
                }

                //- 
                //- Do we have enough space for the data?
                //- 
                if ((buffer.Length - bufferNextWriteIndex) >= packet.Data.Length)
                {
                    for (int i = 0; i < packet.Data.Length; i++)
                    {
                        buffer[bufferNextWriteIndex++] = packet.Data[i];
                    }

                    previousDataStartIndex = bufferStartIndex;
                    previousNextWriteIndex = bufferNextWriteIndex;
                }
                //- 
                //- No, we do not have enough space for data.
                //- 
                else
                {
                    byte[] newBuffer = new byte[buffer.Length + packet.Data.Length];
                    int newBufferStartIndex = 0;
                    int newBufferNextWriteIndex = 0;

                    // Copy old data
                    for (int i = 0; i < bufferNextWriteIndex; i++)
                    {
                        newBuffer[newBufferNextWriteIndex++] = buffer[i];
                    }

                    // Copy new data
                    for (int i = 0; i < packet.Data.Length; i++)
                    {
                        newBuffer[newBufferNextWriteIndex++] = packet.Data[i];
                    }

                    previousData = newBuffer;
                    previousDataStartIndex = newBufferStartIndex;
                    previousNextWriteIndex = newBufferNextWriteIndex;
                }
            }
        }

        internal byte[] GetSingleMessage()
        {
            lock (mutex)
            {
                //- 
                //- Do we have any data available?
                //- 
                if (previousData != null)
                {
                    byte[] buffer = previousData;
                    int bufferStartIndex = previousDataStartIndex;
                    int bufferNextWriteIndex = previousNextWriteIndex;

                    //- 
                    //- Do we have full header?
                    //- 

                    int index = bufferStartIndex;

                    while (index < (bufferNextWriteIndex - 1))
                    {
                        // Finding '##'

                        if (buffer[index] == '#' &&
                            buffer[index + 1] == '#')
                        {
                            bufferStartIndex = index;
                            int readableBytes = bufferNextWriteIndex - index;

                            if (readableBytes >= EncDecMessageHeader.GetMessageHeaderSizeOnBuffer())
                            {
                                int dataBytes = buffer.ReadTotalBytesInMessage(ref index);
                                int availableBytes = bufferNextWriteIndex - index;

                                if (availableBytes >= dataBytes)
                                {
                                    byte[] packet = new byte[dataBytes];

                                    for (int i = 0; i < dataBytes; i++)
                                    {
                                        packet[i] = buffer[index++];
                                    }

                                    previousDataStartIndex = index;

                                    return packet;
                                }
                            }

                            break;
                        }

                        index++;
                    }

                    return null!;
                }
                //- 
                //- No, we do not have any data available.
                //- 
                else
                {
                    return null!;
                }
            }
        }
    }
}
