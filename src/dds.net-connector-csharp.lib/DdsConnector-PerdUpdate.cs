using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Interfaces.NetworkClient;
using DDS.Net.Connector.Types.Enumerations;
using DDS.Net.Connector.Types.Variables;
using System.Text;

namespace DDS.Net.Connector
{
    public partial class DdsConnector
    {
        private void DoPeriodicUpdate(Periodicity periodicity)
        {
            if (periodicity == Periodicity.Normal)
            {
                RegisterAwaitingVariablesWithServer();
            }

            lock (variablesMutex)
            {
                List<BaseVariable> refreshed = new();

                if (periodicity == Periodicity.High)
                {
                    foreach (KeyValuePair<string, BaseVariable> v in uploadVariables)
                    {
                        if (v.Value.Periodicity == periodicity ||
                            v.Value.Periodicity == Periodicity.OnChange)
                        {
                            if (v.Value.RefreshValue())
                            {
                                refreshed.Add(v.Value);
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, BaseVariable> v in uploadVariables)
                    {
                        if (v.Value.Periodicity == periodicity)
                        {
                            if (v.Value.RefreshValue())
                            {
                                refreshed.Add(v.Value);
                            }
                        }
                    }
                }

                SendUpdatedValuesToServer(refreshed);
            }
        }

        private void SendUpdatedValuesToServer(List<BaseVariable> vals)
        {
            int sizeRequired = 0;

            foreach (BaseVariable v in vals)
            {
                sizeRequired += v.GetSizeOnBuffer();
            }

            if (sizeRequired > 0)
            {
                sizeRequired += PacketId.VariablesUpdateAtServer.GetSizeOnBuffer();

                byte[] buffer = new byte[sizeRequired];
                int bufferOffset = 0;

                buffer.WritePacketId(ref bufferOffset, PacketId.VariablesUpdateAtServer);

                foreach (BaseVariable v in vals)
                {
                    v.WriteOnBuffer(ref buffer, ref bufferOffset);
                }

                DataToServer.Enqueue(new PacketToServer(buffer, bufferOffset));
            }
        }

        private void RegisterAwaitingVariablesWithServer()
        {
            lock (variablesMutex)
            {
                int sizeRequired = 0;

                foreach (KeyValuePair<string, BaseVariable> v in uploadVariablesToBeRegistered)
                {
                    sizeRequired +=
                        2 + Encoding.Unicode.GetBytes(v.Key).Length + // Variable name
                        Periodicity.Normal.GetSizeOnBuffer() +        // Periodicity
                        1 +                                           // Provider/Consumer
                        1;                                            // Register/Unregister
                }

                foreach (KeyValuePair<string, BaseVariable> v in downloadVariablesToBeRegistered)
                {
                    sizeRequired +=
                        2 + Encoding.Unicode.GetBytes(v.Key).Length + // Variable name
                        Periodicity.Normal.GetSizeOnBuffer() +        // Periodicity
                        1 +                                           // Provider/Consumer
                        1;                                            // Register/Unregister
                }

                if (sizeRequired > 0)
                {
                    sizeRequired += PacketId.VariablesRegistration.GetSizeOnBuffer();

                    byte[] buffer = new byte[sizeRequired];
                    int bufferOffset = 0;

                    buffer.WritePacketId(ref bufferOffset, PacketId.VariablesRegistration);

                    foreach (KeyValuePair<string, BaseVariable> v in uploadVariablesToBeRegistered)
                    {
                        buffer.WriteString(ref bufferOffset, v.Key);
                        buffer.WritePeriodicity(ref bufferOffset, v.Value.Periodicity);
                        buffer.WriteBoolean(ref bufferOffset, true); // Client is provider of data
                        buffer.WriteBoolean(ref bufferOffset, true); // Do register
                    }

                    foreach (KeyValuePair<string, BaseVariable> v in downloadVariablesToBeRegistered)
                    {
                        buffer.WriteString(ref bufferOffset, v.Key);
                        buffer.WritePeriodicity(ref bufferOffset, v.Value.Periodicity);
                        buffer.WriteBoolean(ref bufferOffset, false); // Client is consumer of data
                        buffer.WriteBoolean(ref bufferOffset, true); // Do register
                    }

                    DataToServer.Enqueue(new PacketToServer(buffer, bufferOffset));
                }
            }
        }

        private void UnregisterVariablesFromServer()
        {
            lock (variablesMutex)
            {
                int sizeRequired = 0;

                foreach (KeyValuePair<string, BaseVariable> v in uploadVariables)
                {
                    sizeRequired +=
                        2 + Encoding.Unicode.GetBytes(v.Key).Length + // Variable name
                        Periodicity.Normal.GetSizeOnBuffer() +        // Periodicity
                        1 +                                           // Provider/Consumer
                        1;                                            // Register/Unregister
                }

                foreach (KeyValuePair<string, BaseVariable> v in downloadVariables)
                {
                    sizeRequired +=
                        2 + Encoding.Unicode.GetBytes(v.Key).Length + // Variable name
                        Periodicity.Normal.GetSizeOnBuffer() +        // Periodicity
                        1 +                                           // Provider/Consumer
                        1;                                            // Register/Unregister
                }

                if (sizeRequired > 0)
                {
                    sizeRequired += PacketId.VariablesRegistration.GetSizeOnBuffer();

                    byte[] buffer = new byte[sizeRequired];
                    int bufferOffset = 0;

                    buffer.WritePacketId(ref bufferOffset, PacketId.VariablesRegistration);

                    foreach (KeyValuePair<string, BaseVariable> v in uploadVariables)
                    {
                        buffer.WriteString(ref bufferOffset, v.Key);
                        buffer.WritePeriodicity(ref bufferOffset, v.Value.Periodicity);
                        buffer.WriteBoolean(ref bufferOffset, true); // Client is provider of data
                        buffer.WriteBoolean(ref bufferOffset, false); // Do register
                    }

                    foreach (KeyValuePair<string, BaseVariable> v in downloadVariables)
                    {
                        buffer.WriteString(ref bufferOffset, v.Key);
                        buffer.WritePeriodicity(ref bufferOffset, v.Value.Periodicity);
                        buffer.WriteBoolean(ref bufferOffset, false); // Client is consumer of data
                        buffer.WriteBoolean(ref bufferOffset, false); // Do register
                    }

                    DataToServer.Enqueue(new PacketToServer(buffer, bufferOffset));
                }
            }
        }
    }
}
