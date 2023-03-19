using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;
using DDS.Net.Connector.Types.Variables;

namespace DDS.Net.Connector
{
    public partial class DdsConnector
    {
        private void ParsePacket(byte[] data)
        {
            int offset = 0;

            try
            {
                PacketId packetId = data.ReadPacketId(ref offset);

                switch (packetId)
                {
                    case PacketId.HandShake:
                        string serverName = data.ReadString(ref offset);
                        string serverVersion = data.ReadString(ref offset);

                        Logger.Info($"Server = {serverName} v{serverVersion}");
                        break;

                    case PacketId.VariablesRegistration:
                        ParseVariablesRegistration(ref data, ref offset);
                        break;

                    case PacketId.VariablesUpdateAtServer:
                        break;

                    case PacketId.VariablesUpdateFromServer:
                        ParseVariablesUpdateFromServer(ref data, ref offset);
                        break;

                    case PacketId.ErrorResponseFromServer:
                        string errorMessage = data.ReadString(ref offset);
                        Logger.Error($"Server Error: {errorMessage}");
                        break;

                    case PacketId.UnknownPacket:
                        Logger.Error($"Unknown message received from the server.");
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Packet parsing error: {e.Message}");
            }
        }

        private void ParseVariablesRegistration(ref byte[] data, ref int offset)
        {
            lock (variablesMutex)
            {
                while (offset < data.Length)
                {
                    (string variableName, ushort variableId, bool isRegister) =
                                    ReadVariableRegistrationElements(ref data, ref offset);

                    if (isRegister)
                    {
                        if (downloadVariablesToBeRegistered.ContainsKey(variableName))
                        {
                            BaseVariable theVar = downloadVariablesToBeRegistered[variableName];
                            theVar.AssignId(variableId);

                            downloadVariables.Add(variableName, theVar);
                            downloadVariablesToBeRegistered.Remove(variableName);
                        }
                        else if (uploadVariablesToBeRegistered.ContainsKey(variableName))
                        {
                            BaseVariable theVar = uploadVariablesToBeRegistered[variableName];
                            theVar.AssignId(variableId);

                            uploadVariables.Add(variableName, theVar);
                            uploadVariablesToBeRegistered.Remove(variableName);
                        }
                    }
                    else
                    {
                        if (downloadVariables.ContainsKey(variableName))
                        {
                            BaseVariable theVar = downloadVariables[variableName];
                            theVar.Reset();

                            downloadVariablesToBeRegistered.Add(variableName, theVar);
                            downloadVariables.Remove(variableName);
                        }
                        else if (uploadVariables.ContainsKey(variableName))
                        {
                            BaseVariable theVar = uploadVariables[variableName];
                            theVar.Reset();

                            uploadVariablesToBeRegistered.Add(variableName, theVar);
                            uploadVariables.Remove(variableName);
                        }
                    }
                }
            }
        }

        private static (string variableName, ushort variableId, bool isRegister)
            ReadVariableRegistrationElements(ref byte[] data, ref int offset)
        {
            string variableName = data.ReadString(ref offset);
            ushort variableId = data.ReadUnsignedWord(ref offset);
            bool isRegister = data.ReadBoolean(ref offset);

            return (variableName, variableId, isRegister);
        }

        private void ParseVariablesUpdateAtServer(ref byte[] data, ref int offset)
        {
            lock (variablesMutex)
            {
                while (offset < data.Length)
                {
                    (ushort variableId, string errorMessage) =
                        ReadVariablesUpdateAtServerElements(ref data, ref offset);

                    foreach (BaseVariable v in uploadVariables.Values)
                    {
                        if (v.Id == variableId)
                        {
                            Logger.Error(
                                $"Variable {v.Name} cannot be updated at the server - " +
                                $"{errorMessage}");
                            break;
                        }
                    }
                }
            }
        }

        private static (ushort variableId, string errorMessage)
            ReadVariablesUpdateAtServerElements(ref byte[] data, ref int offset)
        {
            ushort variableId = data.ReadUnsignedWord(ref offset);
            string errorMessage = data.ReadString(ref offset);

            return (variableId, errorMessage);
        }

        private void ParseVariablesUpdateFromServer(ref byte[] data, ref int offset)
        {
            lock (variablesMutex)
            {
                while (offset < data.Length) ;
            }
        }
    }
}
