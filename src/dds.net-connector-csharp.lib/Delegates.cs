namespace DDS.Net.Connector
{
    public delegate string StringProvider(string variableName);
    public delegate bool BooleanProvider(string variableName);

    public delegate void StringConsumer(string variableName, string variableValue);
    public delegate void BooleanConsumer(string variableName, bool variableValue);
}
