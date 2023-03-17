namespace DDS.Net.Connector.Interfaces.DefaultLogger
{
    /// <summary>
    /// Class <c>ColoredConsoleExtension</c> provides extension methods for
    /// simple printing of colored console messages.
    /// </summary>
    public static class ColoredConsoleExtension
    {
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// Prints a message line using given colors on standard console.
        /// </summary>
        /// <param name="text">Line text.</param>
        /// <param name="fgColor">Text color.</param>
        /// <param name="bgColor">Background color for the text.</param>
        public static void PrintConsoleLine(
            this string text,
            ConsoleColor fgColor = ConsoleColor.White,
            ConsoleColor bgColor = ConsoleColor.Black)
        {
            lock (mutex)
            {
                ConsoleColor beforeFG = Console.ForegroundColor;
                ConsoleColor beforeBG = Console.BackgroundColor;

                Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;

                Console.WriteLine(text);

                Console.ForegroundColor = beforeFG;
                Console.BackgroundColor = beforeBG;
            }
        }
        /// <summary>
        /// Prints text without endline using given colors on standard console.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fgColor">Text color.</param>
        /// <param name="bgColor">Background color for the text.</param>
        public static void PrintConsole(
            this string text,
            ConsoleColor fgColor = ConsoleColor.White,
            ConsoleColor bgColor = ConsoleColor.Black)
        {
            lock (mutex)
            {
                ConsoleColor beforeFG = Console.ForegroundColor;
                ConsoleColor beforeBG = Console.BackgroundColor;

                Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;

                Console.Write(text);

                Console.ForegroundColor = beforeFG;
                Console.BackgroundColor = beforeBG;
            }
        }
    }

    /// <summary>
    /// Class <c>ConsoleLogger</c> implements <c>ILogger</c> interface, and writes
    /// log messages on standard console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel logLevel;

        private readonly ConsoleColor informationTextColor;
        private readonly ConsoleColor informationBackgroundColor;
        private readonly ConsoleColor warningTextColor;
        private readonly ConsoleColor warningBackgroundColor;
        private readonly ConsoleColor errorTextColor;
        private readonly ConsoleColor errorBackgroundColor;

        /// <summary>
        /// Initializes class <c>ConsoleLogger</c> to write log messages on standard console.
        /// </summary>
        /// <param name="logLevel">Minimum log level.</param>
        /// <param name="informationTextColor">Text color for information-level messages.</param>
        /// <param name="informationBackgroundColor">Background color for information-level messages.</param>
        /// <param name="warningTextColor">Text color for warning-level messages.</param>
        /// <param name="warningBackgroundColor">Background color for warning-level messages.</param>
        /// <param name="errorTextColor">Text color for error-level messages.</param>
        /// <param name="errorBackgroundColor">Background color for error-level messages.</param>
        public ConsoleLogger(
            LogLevel logLevel = LogLevel.Information,

            ConsoleColor informationTextColor = ConsoleColor.DarkGray,
            ConsoleColor informationBackgroundColor = ConsoleColor.Black,
            ConsoleColor warningTextColor = ConsoleColor.Yellow,
            ConsoleColor warningBackgroundColor = ConsoleColor.Black,
            ConsoleColor errorTextColor = ConsoleColor.Magenta,
            ConsoleColor errorBackgroundColor = ConsoleColor.Black)
        {
            this.logLevel = logLevel;

            this.informationTextColor = informationTextColor;
            this.informationBackgroundColor = informationBackgroundColor;
            this.warningTextColor = warningTextColor;
            this.warningBackgroundColor = warningBackgroundColor;
            this.errorTextColor = errorTextColor;
            this.errorBackgroundColor = errorBackgroundColor;
        }

        public void Error(string message)
        {
            lock (this)
            {
                $"Error: {message}".PrintConsoleLine(errorTextColor, errorBackgroundColor);
            }
        }

        public void Info(string message)
        {
            if (logLevel == LogLevel.Information)
            {
                lock (this)
                {
                    message.PrintConsoleLine(informationTextColor, informationBackgroundColor);
                }
            }
        }

        public void Warning(string message)
        {
            if (logLevel != LogLevel.Error)
            {
                lock (this)
                {
                    $"Warning: {message}".PrintConsoleLine(warningTextColor, warningBackgroundColor);
                }
            }
        }
    }
}
