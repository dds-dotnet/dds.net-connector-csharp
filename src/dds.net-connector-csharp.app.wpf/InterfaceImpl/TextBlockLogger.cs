using DDS.Net.Connector.Interfaces;
using System.Windows.Controls;

namespace DDS.Net.Connector.WpfApp.InterfaceImpl
{
    internal class TextBlockLogger : ILogger
    {
        public TextBlockLogger(TextBlock textBlock, ScrollViewer scrollViewer, LogLevel logLevel)
        {
            TextBlock = textBlock;
            ScrollViewer = scrollViewer;
            LogLevel = logLevel;
        }

        public TextBlock TextBlock { get; }
        public ScrollViewer ScrollViewer { get; }
        public LogLevel LogLevel { get; }

        public void Error(string message)
        {
            lock (this)
            {
                TextBlock.Dispatcher.Invoke(() =>
                {
                    TextBlock.Text += $"Error: {message}\n";
                    ScrollViewer.ScrollToBottom();
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
                        ScrollViewer.ScrollToBottom();
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
                        ScrollViewer.ScrollToBottom();
                    });
                }
            }
        }
    }
}
