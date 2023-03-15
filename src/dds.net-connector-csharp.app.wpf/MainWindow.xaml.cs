using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces.DefaultLogger;
using DDS.Net.Connector.WpfApp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DDS.Net.Connector.WpfApp
{
    public partial class MainWindow : Window
    {
        private FileLogger logger = new(AppConstants.LOG_FILENAME);
        private DdsConnector connector;

        public MainWindow()
        {
            InitializeComponent();

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

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            connector.Stop();
            logger.Dispose();
        }
    }
}
