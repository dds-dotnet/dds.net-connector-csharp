using DDS.Net.Connector.Interfaces;
using System.Windows.Controls;

namespace DDS.Net.Connector.WpfApp.InterfaceImpl
{
    internal class TextBlockLogger : ILogger
    {
        public TextBlockLogger(TextBlock textBlock, LogLevel logLevel)
        {
            TextBlock = textBlock;
            LogLevel = logLevel;
        }

        public TextBlock TextBlock { get; }
        public LogLevel LogLevel { get; }

        public void Error(string message)
        {
            lock (this)
            {
                TextBlock.Dispatcher.Invoke(() =>
                {
                    TextBlock.Text += $"Error: {message}\n";
                });
            }
        }

        public void Info(string message)
        {
            if (LogLevel == LogLevel.Information)
            {
                lock (this)
                {
                    TextBlock.Dispatcher.Invoke(() =>
                    {
                        TextBlock.Text += $"{message}\n";
                    });
                }
            }
        }

        public void Warning(string message)
        {
            if (LogLevel != LogLevel.Error)
            {
                lock (this)
                {
                    TextBlock.Dispatcher.Invoke(() =>
                    {
                        TextBlock.Text += $"Warning: {message}\n";
                    });
                }
            }
        }
    }
}
