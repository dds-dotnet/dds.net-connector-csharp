namespace DDS.Net.Connector
{
    //- 
    //- Providers
    //- 
    public delegate string StringProvider(string variableName);

    public delegate bool BooleanProvider(string variableName);

    public delegate sbyte ByteProvider(string variableName);
    public delegate short WordProvider(string variableName);
    public delegate int DWordProvider(string variableName);
    public delegate long QWordProvider(string variableName);

    public delegate byte UnsignedByteProvider(string variableName);
    public delegate ushort UnsignedWordProvider(string variableName);
    public delegate uint UnsignedDWordProvider(string variableName);
    public delegate ulong UnsignedQWordProvider(string variableName);

    public delegate float SingleProvider(string variableName);
    public delegate double DoubleProvider(string variableName);

    public delegate byte[] RawBytesProvider(string variableName);

    //- 
    //- Consumers
    //- 
    public delegate void StringConsumer(string variableName, string variableValue);

    public delegate void BooleanConsumer(string variableName, bool variableValue);

    public delegate void ByteConsumer(string variableName, sbyte variableValue);
    public delegate void WordConsumer(string variableName, short variableValue);
    public delegate void DWordConsumer(string variableName, int variableValue);
    public delegate void QWordConsumer(string variableName, long variableValue);

    public delegate void UnsignedByteConsumer(string variableName, byte variableValue);
    public delegate void UnsignedWordConsumer(string variableName, ushort variableValue);
    public delegate void UnsignedDWordConsumer(string variableName, uint variableValue);
    public delegate void UnsignedQWordConsumer(string variableName, ulong variableValue);

    public delegate void SingleConsumer(string variableName, float variableValue);
    public delegate void DoubleConsumer(string variableName, double variableValue);

    public delegate void RawBytesConsumer(string variableName, byte[] variableValue);
}
