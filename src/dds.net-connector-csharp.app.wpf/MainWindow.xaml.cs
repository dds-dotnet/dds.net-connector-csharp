using DDS.Net.Connector.Helpers;
using DDS.Net.Connector.Interfaces;
using DDS.Net.Connector.Interfaces.DefaultLogger;
using DDS.Net.Connector.WpfApp.Configuration;
using DDS.Net.Connector.WpfApp.InterfaceImpl;
using System;
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

            connector.RegisterDoubleProvider("Circle-X", CircleValuesProvider, Types.Enumerations.Periodicity.Highest);
            connector.RegisterDoubleProvider("Circle-Y", CircleValuesProvider, Types.Enumerations.Periodicity.Highest);

            connector.RegisterDoubleConsumer("Circle-X", CircleValuesConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterDoubleConsumer("Circle-Y", CircleValuesConsumer, Types.Enumerations.Periodicity.OnChange);
            

            connector.RegisterStringConsumer("Test-String", TestStringConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterBooleanConsumer("Test-Boolean", TestBooleanConsumer, Types.Enumerations.Periodicity.OnChange);

            connector.RegisterByteConsumer("Test-Byte", TestByteConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterWordConsumer("Test-Word", TestWordConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterDWordConsumer("Test-DWord", TestDWordConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterQWordConsumer("Test-QWord", TestQWordConsumer, Types.Enumerations.Periodicity.OnChange);

            connector.RegisterUnsignedByteConsumer("Test-UnsignedByte", TestUnsignedByteConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterUnsignedWordConsumer("Test-UnsignedWord", TestUnsignedWordConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterUnsignedDWordConsumer("Test-UnsignedDWord", TestUnsignedDWordConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterUnsignedQWordConsumer("Test-UnsignedQWord", TestUnsignedQWordConsumer, Types.Enumerations.Periodicity.OnChange);
            
            connector.RegisterSingleConsumer("Test-Single", TestSingleConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterDoubleConsumer("Test-Double", TestDoubleConsumer, Types.Enumerations.Periodicity.OnChange);

            connector.RegisterRawBytesConsumer("Test-Bytes", TestBytesConsumer, Types.Enumerations.Periodicity.OnChange);
        }

        private void TestStringConsumer(string variableName, string variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testStringValue.Text = variableValue; });
        }
        private void TestBooleanConsumer(string variableName, bool variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testBooleanValue.Text = $"{variableValue}"; });
        }
        private void TestByteConsumer(string variableName, sbyte variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testByteValue.Text = $"{variableValue}"; });
        }
        private void TestWordConsumer(string variableName, short variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testWordValue.Text = $"{variableValue}"; });
        }
        private void TestDWordConsumer(string variableName, int variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testDWordValue.Text = $"{variableValue}"; });
        }
        private void TestQWordConsumer(string variableName, long variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testQWordValue.Text = $"{variableValue}"; });
        }
        private void TestUnsignedByteConsumer(string variableName, byte variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testUnsignedByteValue.Text = $"{variableValue}"; });
        }
        private void TestUnsignedWordConsumer(string variableName, ushort variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testUnsignedWordValue.Text = $"{variableValue}"; });
        }
        private void TestUnsignedDWordConsumer(string variableName, uint variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testUnsignedDWordValue.Text = $"{variableValue}"; });
        }
        private void TestUnsignedQWordConsumer(string variableName, ulong variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testUnsignedQWordValue.Text = $"{variableValue}"; });
        }
        private void TestSingleConsumer(string variableName, float variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testSingleValue.Text = $"{variableValue}"; });
        }
        private void TestDoubleConsumer(string variableName, double variableValue)
        {
            mainWindow.Dispatcher.Invoke(() => { testDoubleValue.Text = $"{variableValue}"; });
        }
        private void TestBytesConsumer(string variableName, byte[] variableValue)
        {
            string s = "";
            foreach (byte b in variableValue)
            {
                s += (int)b;
                s += " | ";
            }

            mainWindow.Dispatcher.Invoke(() => { testBytesValue.Text = s; });
        }


        private readonly double circleWidth;
        private readonly double circleHeight;

        double circleX = 0;
        double circleY = 0;

        private void CircleValuesConsumer(string variableName, double variableValue)
        {
            if (variableName == "Circle-X")
            {
                circleX = variableValue;
            }
            else if (variableName == "Circle-Y")
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

        private double CircleValuesProvider(string variableName)
        {
            if (variableName == "Circle-X")
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
            if (variableName == "Circle-Y")
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
