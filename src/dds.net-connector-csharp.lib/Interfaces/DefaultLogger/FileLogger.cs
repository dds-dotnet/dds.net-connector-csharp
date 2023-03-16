using DDS.Net.Connector.Helpers;

namespace DDS.Net.Connector.Interfaces.DefaultLogger
{
    /// <summary>
    /// Class <c>FileLogger</c> implements <c>ILogger</c> interface, and dumps
    /// log messages into a file.
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {
        private readonly LogLevel _logLevel;
        private StreamWriter? _writer;

        /// <summary>
        /// Initializes class <c>FileLogger</c> to dump log messages into given file name.
        /// </summary>
        /// <param name="filename">Log file name.</param>
        /// <param name="logLevel">Minimum log level.</param>
        /// <exception cref="Exception"></exception>
        public FileLogger(string filename, LogLevel logLevel = LogLevel.Information)
        {
            try
            {
                filename.CreateFoldersForRelativeFilename();

                _writer = File.AppendText(filename);

                _writer.WriteLine($"DDS.Net Connector v{DdsConnector.LibraryVersion}");
                _writer.WriteLine($"- Starting at {DateTime.Now:yyyy/MMM/dd - hh:mm:ss tt}");
                _writer.WriteLine($"------------------------------------------------------------------");
                _writer.WriteLine($"");

                _writer.Flush();
                _writer.AutoFlush = true;
            }
            catch (Exception ex)
            {
                _writer = null;
                throw new Exception($"Cannot write log file \"{filename}\" - {ex.Message}");
            }

            _logLevel = logLevel;
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                lock (this)
                {
                    _writer.WriteLine($"");
                    _writer.WriteLine($"------------------------------------------------------------------");
                    _writer.WriteLine($"- Stopping at {DateTime.Now:yyyy/MMM/dd - hh:mm:ss tt}");
                    _writer.WriteLine($"------------------------------------------------------------------");
                    _writer.WriteLine($"");

                    _writer.Flush();
                    _writer.Dispose();
                    _writer = null;
                }
            }
        }

        public void Error(string message)
        {
            lock (this)
            {
                _writer?.WriteLine($"Error: {message}");
            }
        }

        public void Info(string message)
        {
            if (_logLevel == LogLevel.Information)
            {
                lock (this)
                {
                    _writer?.WriteLine(message);
                }
            }
        }

        public void Warning(string message)
        {
            if (_logLevel != LogLevel.Error)
            {
                lock (this)
                {
                    _writer?.WriteLine($"Warning: {message}");
                }
            }
        }
    }
}
