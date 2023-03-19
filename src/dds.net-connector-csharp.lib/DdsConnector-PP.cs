using DDS.Net.Connector.EncodersAndDecoders;
using DDS.Net.Connector.Types.Enumerations;
using DDS.Net.Connector.Types.Variables;
using DDS.Net.Connector.Types.Variables.Primitives;
using DDS.Net.Connector.Types.Variables.RawBytes;

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
                        ParseVariablesUpdateAtServer(ref data, ref offset);
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
        #region Variables' Registration
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
        #endregion
        #region Variables' Update at Server
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
        #endregion
        #region Variables' Update from Server
        private void ParseVariablesUpdateFromServer(ref byte[] data, ref int offset)
        {
            List<BaseVariable> updatedVariables = new();

            lock (variablesMutex)
            {
                Periodicity periodicity = data.ReadPeriodicity(ref offset);

                while (offset < data.Length)
                {
                    ushort id = data.ReadUnsignedWord(ref offset);
                    VariableType mainType = data.ReadVariableType(ref offset);
                    BaseVariable var = null!;

                    foreach (BaseVariable v in downloadVariables.Values)
                    {
                        if (id == v.Id)
                        {
                            var = v;
                            break;
                        }
                    }

                    /************************************************************************/
                    /*                                                                      */
                    /* Processing RawBytes variable                                         */
                    /*                                                                      */
                    /************************************************************************/
                    if (mainType == VariableType.RawBytes)
                    {
                        int totalBytes = (int)data.ReadUnsignedDWord(ref offset);

                        //- 
                        //- The input value is not null
                        //- 
                        if (totalBytes > 0)
                        {
                            //- 
                            //- Not enough data is available
                            //- 
                            if (totalBytes + offset >= data.Length)
                            {
                                if (var != null)
                                {
                                    throw new Exception($"Insufficient data provided for {var.Name}");
                                }
                                else
                                {
                                    throw new Exception($"Insufficient data provided for unknown RawBytes variable");
                                }
                            }

                            //- 
                            //- Data is available and the types are matching
                            //- 
                            if (var is RawBytesVariable rbv)
                            {
                                byte[] bytes = new byte[totalBytes];

                                for (int i = 0; i < totalBytes; i++)
                                {
                                    bytes[i] = data[offset++];
                                }

                                if (rbv.UpdateData(bytes))
                                {
                                    updatedVariables.Add(var);
                                }
                            }
                            //- 
                            //- Data is available but the types are not matching
                            //- 
                            else
                            {
                                offset += totalBytes;
                            }
                        }
                        //- 
                        //- The input value is null
                        //- 
                        else
                        {
                            if (var is RawBytesVariable rbv)
                            {
                                if (rbv.UpdateData(null!))
                                {
                                    updatedVariables.Add(var);
                                }
                            }
                        }
                    }
                    /************************************************************************/
                    /*                                                                      */
                    /* Processing Primitive variable                                         */
                    /*                                                                      */
                    /************************************************************************/
                    else if (mainType == VariableType.Primitive)
                    {
                        //- 
                        //- Discarding the value when we do not have locally registered variable.
                        //- 
                        if (var == null || var is not BasePrimitive)
                        {
                            PrimitiveType pt = data.ReadPrimitiveType(ref offset);

                            switch (pt)
                            {
                                case PrimitiveType.String:        data.ReadString(ref offset);        break;
                                case PrimitiveType.Boolean:       data.ReadBoolean(ref offset);       break;
                                case PrimitiveType.Byte:          data.ReadByte(ref offset);          break;
                                case PrimitiveType.Word:          data.ReadWord(ref offset);          break;
                                case PrimitiveType.DWord:         data.ReadDWord(ref offset);         break;
                                case PrimitiveType.QWord:         data.ReadQWord(ref offset);         break;
                                case PrimitiveType.UnsignedByte:  data.ReadUnsignedByte(ref offset);  break;
                                case PrimitiveType.UnsignedWord:  data.ReadUnsignedWord(ref offset);  break;
                                case PrimitiveType.UnsignedDWord: data.ReadUnsignedDWord(ref offset); break;
                                case PrimitiveType.UnsignedQWord: data.ReadUnsignedQWord(ref offset); break;
                                case PrimitiveType.Single:        data.ReadSingle(ref offset);        break;
                                case PrimitiveType.Double:        data.ReadDouble(ref offset);        break;
                                case PrimitiveType.UnknownPrimitiveType:                              break;
                            }

                            if (var != null && var is not BasePrimitive)
                            {
                                Logger.Error($"Cannot assign a {pt} value to a non-primitive local variable");
                            }
                        }

                        //- 
                        //- Processing the variable value when we have local primitive variable.
                        //- 
                        if (var != null && var is BasePrimitive bpv)
                        {
                            PrimitiveType pt = data.ReadPrimitiveType(ref offset);
                            bool valueUpdated = false;

                            switch (pt)
                            {
                                case PrimitiveType.String:        valueUpdated = UpdatePrimitiveVariableWithString(bpv, data.ReadString(ref offset));               break;
                                case PrimitiveType.Boolean:       valueUpdated = UpdatePrimitiveVariableWithBoolean(bpv, data.ReadBoolean(ref offset));             break;
                                case PrimitiveType.Byte:          valueUpdated = UpdatePrimitiveVariableWithByte(bpv, data.ReadByte(ref offset));                   break;
                                case PrimitiveType.Word:          valueUpdated = UpdatePrimitiveVariableWithWord(bpv, data.ReadWord(ref offset));                   break;
                                case PrimitiveType.DWord:         valueUpdated = UpdatePrimitiveVariableWithDWord(bpv, data.ReadDWord(ref offset));                 break;
                                case PrimitiveType.QWord:         valueUpdated = UpdatePrimitiveVariableWithQWord(bpv, data.ReadQWord(ref offset));                 break;
                                case PrimitiveType.UnsignedByte:  valueUpdated = UpdatePrimitiveVariableWithUnsignedByte(bpv, data.ReadUnsignedByte(ref offset));   break;
                                case PrimitiveType.UnsignedWord:  valueUpdated = UpdatePrimitiveVariableWithUnsignedWord(bpv, data.ReadUnsignedWord(ref offset));   break;
                                case PrimitiveType.UnsignedDWord: valueUpdated = UpdatePrimitiveVariableWithUnsignedDWord(bpv, data.ReadUnsignedDWord(ref offset)); break;
                                case PrimitiveType.UnsignedQWord: valueUpdated = UpdatePrimitiveVariableWithUnsignedQWord(bpv, data.ReadUnsignedQWord(ref offset)); break;
                                case PrimitiveType.Single:        valueUpdated = UpdatePrimitiveVariableWithSingle(bpv, data.ReadSingle(ref offset));               break;
                                case PrimitiveType.Double:        valueUpdated = UpdatePrimitiveVariableWithDouble(bpv, data.ReadDouble(ref offset));               break;
                                case PrimitiveType.UnknownPrimitiveType:                                                                                            break;
                            }

                            if (valueUpdated)
                            {
                                updatedVariables.Add(var);
                            }
                        }
                    }

                } // while (offset < data.Length)

            } // lock (variablesMutex)

        }

        private bool UpdatePrimitiveVariableWithString(BasePrimitive bpv, string v)
        {
            if (bpv is StringVariable str)
            {
                if (str.Value != v)
                {
                    str.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                if (bool.TryParse(v, out bool val))
                {
                    if (bl.Value != val)
                    {
                        bl.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is ByteVariable bt)
            {
                if (sbyte.TryParse(v, out sbyte val))
                {
                    if (bt.Value != val)
                    {
                        bt.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is WordVariable wrd)
            {
                if (short.TryParse(v, out short val))
                {
                    if (wrd.Value != val)
                    {
                        wrd.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is DWordVariable dwrd)
            {
                if (int.TryParse(v, out int val))
                {
                    if (dwrd.Value != val)
                    {
                        dwrd.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is QWordVariable qwrd)
            {
                if (long.TryParse(v, out long val))
                {
                    if (qwrd.Value != val)
                    {
                        qwrd.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
                if (byte.TryParse(v, out byte val))
                {
                    if (ubt.Value != val)
                    {
                        ubt.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
                if (ushort.TryParse(v, out ushort val))
                {
                    if (uwrd.Value != val)
                    {
                        uwrd.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
                if (uint.TryParse(v, out uint val))
                {
                    if (udwrd.Value != val)
                    {
                        udwrd.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
                if (ulong.TryParse(v, out ulong val))
                {
                    if (uqwrd.Value != val)
                    {
                        uqwrd.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is SingleVariable sngl)
            {
                if (float.TryParse(v, out float val))
                {
                    if (sngl.Value != val)
                    {
                        sngl.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (bpv is DoubleVariable dbl)
            {
                if (double.TryParse(v, out double val))
                {
                    if (dbl.Value != val)
                    {
                        dbl.Value = val;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            Logger.Error(
                $"Received string {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithBoolean(BasePrimitive bpv, bool v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                if (bl.Value != v)
                {
                    bl.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            Logger.Error(
                $"Received Boolean {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithByte(BasePrimitive bpv, sbyte v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Byte {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
                if (bt.Value != v)
                {
                    bt.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is WordVariable wrd)
            {
                if (wrd.Value != v)
                {
                    wrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is DWordVariable dwrd)
            {
                if (dwrd.Value != v)
                {
                    dwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is QWordVariable qwrd)
            {
                if (qwrd.Value != v)
                {
                    qwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
                if (sngl.Value != v)
                {
                    sngl.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is DoubleVariable dbl)
            {
                if (dbl.Value != v)
                {
                    dbl.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            Logger.Error(
                $"Received Byte {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithWord(BasePrimitive bpv, short v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Word {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
                if (wrd.Value != v)
                {
                    wrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received Word {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithDWord(BasePrimitive bpv, int v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting DWord {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
                if (dwrd.Value != v)
                {
                    dwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received DWord {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithQWord(BasePrimitive bpv, long v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting QWord {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
                if (qwrd.Value != v)
                {
                    qwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received QWord {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithUnsignedByte(BasePrimitive bpv, byte v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Unsigned Byte {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
                if (ubt.Value != v)
                {
                    ubt.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received Unsigned Byte {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithUnsignedWord(BasePrimitive bpv, ushort v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Unsigned Word {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
                if (uwrd.Value != v)
                {
                    uwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received Unsigned Word {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithUnsignedDWord(BasePrimitive bpv, uint v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Unsigned DWord {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
                if (udwrd.Value != v)
                {
                    udwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received Unsigned DWord {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithUnsignedQWord(BasePrimitive bpv, ulong v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Unsigned QWord {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
                if (uqwrd.Value != v)
                {
                    uqwrd.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received Unsigned QWord {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithSingle(BasePrimitive bpv, float v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Single {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
                if (sngl.Value != v)
                {
                    sngl.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is DoubleVariable dbl)
            {
            }

            Logger.Error(
                $"Received Single {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }

        private bool UpdatePrimitiveVariableWithDouble(BasePrimitive bpv, double v)
        {
            if (bpv is StringVariable str)
            {
                string conv = $"{v}";

                if (str.Value != conv)
                {
                    str.Value = conv;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (bpv is BooleanVariable bl)
            {
                bool conv = v != 0;
                bool ret = false;

                if (bl.Value != conv)
                {
                    bl.Value = conv;
                    ret = true;
                }

                Logger.Warning($"Possible loss of received data - converting Double {v} to {bl.GetPrintableTypeName()} {conv}");

                return ret;
            }
            else if (bpv is ByteVariable bt)
            {
            }
            else if (bpv is WordVariable wrd)
            {
            }
            else if (bpv is DWordVariable dwrd)
            {
            }
            else if (bpv is QWordVariable qwrd)
            {
            }
            else if (bpv is UnsignedByteVariable ubt)
            {
            }
            else if (bpv is UnsignedWordVariable uwrd)
            {
            }
            else if (bpv is UnsignedDWordVariable udwrd)
            {
            }
            else if (bpv is UnsignedQWordVariable uqwrd)
            {
            }
            else if (bpv is SingleVariable sngl)
            {
            }
            else if (bpv is DoubleVariable dbl)
            {
                if (dbl.Value != v)
                {
                    dbl.Value = v;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            Logger.Error(
                $"Received Double {v} cannot be assigned to " +
                $"the variable {bpv.Name} of type {bpv.GetPrintableTypeName()}");

            return false;
        }
        #endregion
    }
}
