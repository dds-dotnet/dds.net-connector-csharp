﻿namespace DDS.Net.Connector.Interfaces.DefaultLogger
{
    /// <summary>
    /// Class <c>SplitLogger</c> implements <c>ILogger</c> interface, and
    /// provides a means of splitting log messages to multiple implementations
    /// of <c>ILogger</c> interface.
    /// </summary>
    public class SplitLogger : ILogger, IDisposable
    {
        private readonly ILogger firstLogger;
        private readonly ILogger secondLogger;
        private readonly ILogger[]? loggers;

        /// <summary>
        /// Initializes class <c>SplitLogger</c> for splitting log messages
        /// in at least two <c>ILogger</c> interface implementations.
        /// </summary>
        /// <param name="firstLogger">Required first <c>ILogger</c> interface implementation.</param>
        /// <param name="secondLogger">Required second <c>ILogger</c> interface implementation.</param>
        /// <param name="loggers"><c>ILogger</c> interface implementations.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SplitLogger(ILogger firstLogger, ILogger secondLogger, params ILogger[] loggers)
        {
            this.firstLogger = firstLogger ?? throw new ArgumentNullException(nameof(firstLogger));
            this.secondLogger = secondLogger ?? throw new ArgumentNullException(nameof(secondLogger));
            this.loggers = loggers;
        }

        public void Dispose()
        {
            if (firstLogger is IDisposable firstDisposable) { firstDisposable.Dispose(); }
            if (secondLogger is IDisposable secondDisposable) { secondDisposable.Dispose(); }

            if (loggers != null && loggers.Length > 0)
            {
                foreach (var logger in loggers)
                {
                    if (logger != null && logger is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        public void Error(string message)
        {
            firstLogger.Error(message);
            secondLogger.Error(message);

            if (loggers != null && loggers.Length > 0)
            {
                foreach (var logger in loggers)
                {
                    logger?.Error(message);
                }
            }
        }

        public void Info(string message)
        {
            firstLogger?.Info(message);
            secondLogger?.Info(message);

            if (loggers != null && loggers.Length > 0)
            {
                foreach (var logger in loggers)
                {
                    logger?.Info(message);
                }
            }
        }

        public void Warning(string message)
        {
            firstLogger?.Warning(message);
            secondLogger?.Warning(message);

            if (loggers != null && loggers.Length > 0)
            {
                foreach (var logger in loggers)
                {
                    logger?.Warning(message);
                }
            }
        }
    }
}
