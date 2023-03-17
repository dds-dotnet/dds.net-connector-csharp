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

        private DdsConnector connector;

        public MainWindow()
        {
            InitializeComponent();

            logger = new(
                new FileLogger(AppConstants.LOG_FILENAME, Interfaces.LogLevel.Information),
                new TextBlockLogger(logTextBlock, logScrollViewer, Interfaces.LogLevel.Information));

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
            logger.Dispose();
        }
    }
}
