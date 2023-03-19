﻿using DDS.Net.Connector.Helpers;
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

            logger = new(
                new FileLogger(AppConstants.LOG_FILENAME, Interfaces.LogLevel.Information),
                new TextBlockLogger(logTextBlock, logScrollViewer, Interfaces.LogLevel.Warning));

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

        double maxX = 200;
        double maxY = 200;
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
                    x += 4.5;
                }
                else
                {
                    x -= 4.5;
                }

                if (x >= maxX)
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
                    y += 5.8;
                }
                else
                {
                    y -= 5.8;
                }

                if (y >= maxY)
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
    }
}
