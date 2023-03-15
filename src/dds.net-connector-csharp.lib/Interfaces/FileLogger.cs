using DDS.Net.Connector.Helpers;

namespace DDS.Net.Connector.Interfaces
{
    internal class FileLogger : ILogger, IDisposable
    {
        private readonly LogLevel _logLevel;
        private StreamWriter? _writer;

        public FileLogger(string filename, LogLevel logLevel = LogLevel.Information)
        {
            try
            {
                filename.CreateFoldersForRelativeFilename();

                _writer = File.AppendText(filename);

                _writer.WriteLine($"DDS.Net Connector v{DdsConnector.Version}");
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

        public void Error(string message)
        {
            _writer?.WriteLine($"Error: {message}");
        }

        public void Info(string message)
        {
            if (_logLevel == LogLevel.Information)
            {
                _writer?.WriteLine(message);
            }
        }

        public void Warning(string message)
        {
            if (_logLevel != LogLevel.Error)
            {
                _writer?.WriteLine($"Warning: {message}");
            }
        }
    }
}
