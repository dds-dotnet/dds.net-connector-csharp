namespace DDS.Net.Connector.Interfaces
{
    /// <summary>
    /// Level of log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Status information.
        /// </summary>
        Information,
        /// <summary>
        /// Priority information.
        /// </summary>
        Warning,
        /// <summary>
        /// Critical information.
        /// </summary>
        Error
    }

    /// <summary>
    /// Interface for outputting log messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes <c cref="LogLevel.Information">Information</c>-level message to log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);
        /// <summary>
        /// Writes <c cref="LogLevel.Warning">Warning</c>-level message to log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);
        /// <summary>
        /// Writes <c cref="LogLevel.Error">Error</c>-level message to log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);
    }
}
