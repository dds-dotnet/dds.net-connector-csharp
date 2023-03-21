&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <img src="./.assets/DDS.Net Connector Icon-CS-BG-None.png" width="15%" />


# DDS.Net C# Connector - v1.1.0

*DDS.Net C# Connector* intends to be a lightweight and performant connector for connecting distributed C# applications through *DDS.Net Server*. The supported data types are:

| Main type                                          | Sub-type          | Represented data                                    |
|----------------------------------------------------|-------------------|-----------------------------------------------------|
| ***Primitive*** &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; | *String*          | Sequence of characters in Unicode                   |
|                                                    | *Boolean*         | A boolean (True or False)                           |
|                                                    | *Byte*            | 1-byte Signed Integer                               |
|                                                    | *Word*            | 2-byte Signed Integer                               |
|                                                    | *DWord*           | 4-byte Signed Integer                               |
|                                                    | *QWord*           | 8-byte Signed Integer                               |
|                                                    | *Unsigned Byte*   | 1-byte Unsigned Integer                             |
|                                                    | *Unsigned Word*   | 2-byte Unsigned Integer                             |
|                                                    | *Unsigned DWord*  | 4-byte Unsigned Integer                             |
|                                                    | *Unsigned QWord*  | 8-byte Unsigned Integer                             |
|                                                    | *Single*          | A single precision (4-byte) Floating-point number   |
|                                                    | *Double*          | A double precision (8-byte) Floating-point number   |
| ***Raw Bytes***                                    | -                 | Sequence of bytes                                   |



## Sample application

Using various extensions and helpers available in the library, development of connected application is quite easy. A sample *WPF* application's window goes as follows:

```xml
<!-- MainWindow.xaml -->

<Window x:Name="mainWindow" x:Class="DDS.Net.Connector.WpfApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:DDS.Net.Connector.WpfApp"
        mc:Ignorable="d"
        Title="DDS.Net Connected App" Height="450" Width="800"
        Loaded="OnWindowLoaded" Closing="OnWindowClosing" SizeChanged="OnWindowResized">
    <Grid>
        <Canvas x:Name="theCanvas">
            <Ellipse x:Name="theCircle"
                     Canvas.Left="0" Canvas.Right="0"
                     Width="50" Height="50"
                     Fill="Bisque" />
        </Canvas>
    </Grid>
</Window>
```
The code-behind file:
```csharp
// MainWindow.xaml.cs

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
        private FileLogger logger;
        private DdsConnector connector;

        public MainWindow()
        {
            InitializeComponent();

            circleWidth = theCircle.Width;
            circleHeight = theCircle.Height;

            logger = new FileLogger("my-log-file.log", LogLevel.Information);

            INIConfigIO serverConfig = new("my-config.ini");

            connector = new(
                  mainWindow.Title,
                  serverConfig.GetString("DDS Server/ServerIPv4"),
                  (ushort)serverConfig.GetInteger("DDS Server/ServerPortTCP"),
                  logger);

            connector.RegisterDoubleProvider(
                "TESTX", MyDoubleProvider, Types.Enumerations.Periodicity.Highest);
            connector.RegisterDoubleProvider(
                "TESTY", MyDoubleProvider, Types.Enumerations.Periodicity.Highest);

            connector.RegisterDoubleConsumer(
                "TESTX", MyDoubleConsumer, Types.Enumerations.Periodicity.OnChange);
            connector.RegisterDoubleConsumer(
                "TESTY", MyDoubleConsumer, Types.Enumerations.Periodicity.OnChange);
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
```

Our sample config "*my-config.ini*":
```ini
[DDS Server]
ServerIPv4 = 127.0.0.1
ServerPortTCP = 44556
```



## Library usage

The [library](./docs/usage/README.md) includes *DDS.Net C# Connector* and various helpers to ease developing connected .NET applications.



