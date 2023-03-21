using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces;
using DDS.Net.Connector.Interfaces.DefaultLogger;
using DDS.Net.Connector.WpfApp.Configuration;
using DDS.Net.Connector.WpfApp.InterfaceImpl;
using System.Windows;
using System.Windows.Controls;

namespace DDS.Net.Connector.WpfApp
{
    public partial class MainWindow : Window
    {
        private SplitLogger logger;
        private DdsConnector connector;

        public MainWindow()
        {
            InitializeComponent();

            circleWidth = theCircle.Width;
            circleHeight = theCircle.Height;

            logger = new(
                new FileLogger(AppConstants.LOG_FILENAME, LogLevel.Information),
                new TextBlockLogger(logTextBlock, logScrollViewer, LogLevel.Information));

            INIConfigIO serverConfig = new(AppConstants.SERVER_CONFIG_FILENAME);

            connector = new(
                            mainWindow.Title,
                            serverConfig.GetString("DDS Server/ServerIPv4"),
                            (ushort)serverConfig.GetInteger("DDS Server/ServerPortTCP"),
                            logger);

            connector.RegisterDoubleProvider("TESTX", MyDoubleProvider, Types.Enumerations.Periodicity.Highest);
            connector.RegisterDoubleProvider("TESTY", MyDoubleProvider, Types.Enumerations.Periodicity.Highest);

            connector.RegisterDoubleConsumer("TESTX", MyDoubleConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterDoubleConsumer("TESTY", MyDoubleConsumer, Types.Enumerations.Periodicity.OnChange);
        }

        private readonly double circleWidth;
        private readonly double circleHeight;

        double circleX = 0;
        double circleY = 0;

        private void MyDoubleConsumer(string variableName, double variableValue)
        {
            if (variableName == "TESTX")
            {
                circleX = variableValue;
            }
            else if (variableName == "TESTY")
            {
                circleY = variableValue;

                mainWindow.Dispatcher.Invoke(() =>
                {
                    theCircle.SetValue(Canvas.LeftProperty, circleX);
                    theCircle.SetValue(Canvas.TopProperty, circleY);
                });
            }
        }

        double maxX = 500;
        double maxY = 180;
        double x = 0;
        double y = 0;
        bool isXIncreasing = true;
        bool isYIncreasing = true;

        private double MyDoubleProvider(string variableName)
        {
            if (variableName == "TESTX")
            {
                if (isXIncreasing)
                {
                    x += 10;
                }
                else
                {
                    x -= 10;
                }

                if (x >= (maxX - circleWidth))
                {
                    isXIncreasing = false;
                }
                else if (x <= 0)
                {
                    isXIncreasing = true;
                }

                return x;
            }
            if (variableName == "TESTY")
            {
                if (isYIncreasing)
                {
                    y += 10;
                }
                else
                {
                    y -= 10;
                }

                if (y >= (maxY - circleHeight))
                {
                    isYIncreasing = false;
                }
                else if (y <= 0)
                {
                    isYIncreasing = true;
                }

                return y;
            }

            return 0;
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

        private void OnWindowResized(object sender, SizeChangedEventArgs e)
        {
            maxX = theCanvas.ActualWidth;
            maxY = theCanvas.ActualHeight;
        }
    }
}
