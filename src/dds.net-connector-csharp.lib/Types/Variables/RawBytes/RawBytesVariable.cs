using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;

namespace DDS.Net.Connector.Types.Variables.RawBytes
{
    /// <summary>
    /// Class <c>RawBytesVariable</c> is a sub-class of <c>BaseVariable</c> and
    /// represents <c cref="VariableType.RawBytes">VariableType.RawBytes</c> handling.
    /// </summary>
    internal class RawBytesVariable : BaseVariable
    {
        public byte[] Data { get; private set; } = null!;

        public RawBytesProvider ValueProvider { get; private set; } = null!;
        public event RawBytesConsumer ValueUpdated = null!;

        public RawBytesVariable(
                    string name,
                    Periodicity periodicity,
                    RawBytesProvider rawBytesProvider = null!)

            : base(name, VariableType.RawBytes, periodicity)
        {
            ValueProvider = rawBytesProvider;
        }

        /// <summary>
        /// Updates the data element.
        /// </summary>
        /// <param name="data">New data.</param>
        /// <returns>True = Data is changed; False = Same value as already held.</returns>
        public bool UpdateData(byte[] data)
        {
            if (data == null)
            {
                if (Data == null)
                {
                    return false;
                }
                else
                {
                    Data = null!;
                    return true;
                }
            }
            else
            {
                if (Data == null)
                {
                    Data = data;
                    return true;
                }
                else
                {
                    if (Data.Length != data.Length)
                    {
                        Data = data;
                        return true;
                    }
                    else
                    {
                        bool isDiff = false;

                        for (int i = 0; i < Data.Length; i++)
                        {
                            if (Data[i] != data[i])
                            {
                                isDiff = true;
                            }

                            Data[i] = data[i];
                        }

                        return isDiff;
                    }
                }
            }
        }

        public override int GetSubTypeSizeOnBuffer()
        {
            //- 
            //- We do not have sub-types here unlike the primitive types.
            //- Thereby, its sub-type will take 0-bytes on the data buffer.
            //- 

            return 0;
        }

        public override int GetValueSizeOnBuffer()
        {
            int total = 4; // Number of bytes required for writing array size on the buffer

            if (Data != null)
            {
                total += Data.Length;
            }

            return total;
        }

        public override void WriteSubTypeOnBuffer(ref byte[] buffer, ref int offset)
        {
            //- 
            //- We do not have sub-types here unlike the primitive types.
            //- So, we do not write anything here.
            //- 
        }

        public override void WriteValueOnBuffer(ref byte[] buffer, ref int offset)
        {
            if (offset + GetValueSizeOnBuffer() >= buffer.Length)
            {
                throw new Exception(
                    $"Cannot fit {GetValueSizeOnBuffer()} bytes " +
                    $"at offset {offset} on the buffer with size {buffer.Length}");
            }

            if (Data != null)
            {
                buffer.WriteUnsignedDWord(ref offset, (uint)Data.Length);

                for (int i = 0; i < Data.Length; i++)
                {
                    buffer[offset++] = Data[i];
                }
            }
            else
            {
                buffer.WriteUnsignedDWord(ref offset, 0); // Size is zero here
            }
        }

        public override bool RefreshValue()
        {
            if (ValueProvider != null)
            {
                byte[] newData = ValueProvider(Name);
                return UpdateData(newData);
            }

            return false;
        }
    }
}
