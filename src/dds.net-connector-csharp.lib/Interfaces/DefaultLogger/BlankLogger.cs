namespace DDS.Net.Connector.Interfaces.DefaultLogger
{
    /// <summary>
    /// Class <c>BlankLogger</c> implements <c>ILogger</c> interface, though does nothing
    /// with log messages. Use it to discard all the messages.
    /// </summary>
    public class BlankLogger : ILogger
    {
        public void Error(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Warning(string message)
        {
        }
    }
}
