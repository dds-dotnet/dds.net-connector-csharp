using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces.DefaultLogger;
using DDS.Net.Connector.WpfApp.Configuration;
using DDS.Net.Connector.WpfApp.InterfaceImpl;
using System.Windows;

namespace DDS.Net.Connector.WpfApp
{
    public partial class MainWindow : Window
    {
        private SplitLogger logger;
        private FileLogger fileLogger;
        private TextBlockLogger textBlockLogger;

        private DdsConnector connector;

        public MainWindow()
        {
            InitializeComponent();

            fileLogger = new(AppConstants.LOG_FILENAME, Interfaces.LogLevel.Information);
            textBlockLogger = new TextBlockLogger(logTextBlock, logScrollViewer, Interfaces.LogLevel.Information);
            logger = new(fileLogger, textBlockLogger);

            INIConfigIO serverConfig = new(AppConstants.SERVER_CONFIG_FILENAME);

            connector = new(
                            mainWindow.Title,
                            serverConfig.GetString("DDS Server/ServerIPv4"),
                            (ushort)serverConfig.GetInteger("DDS Server/ServerPortTCP"),
                            logger);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            connector.Start();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connector.Stop();
            fileLogger.Dispose();
        }
    }
}
