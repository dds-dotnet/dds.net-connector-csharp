namespace DDS.Net.Connector
{
    public delegate string StringProvider(string variableName);
    public delegate void StringConsumer(string variableName, string variableValue);
}
