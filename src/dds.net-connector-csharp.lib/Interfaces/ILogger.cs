namespace DDS.Net.Connector.Interfaces
{
    /// <summary>
    /// Level of log messages.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Least important information.
        /// </summary>
        Information,
        /// <summary>
        /// Medium-level of information.
        /// </summary>
        Warning,
        /// <summary>
        /// Critical-level of information.
        /// </summary>
        Error
    }

    /// <summary>
    /// Interface for outputting log messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes <c cref="LogLevel.Information">Information</c>-level messages to log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);
        /// <summary>
        /// Writes <c cref="LogLevel.Warning">Warning</c>-level messages to log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);
        /// <summary>
        /// Writes <c cref="LogLevel.Error">Error</c>-level messages to log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);
    }
}
