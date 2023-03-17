namespace DDS.Net.Connector.Interfaces.DefaultLogger
{
    /// <summary>
    /// Class <c>ColoredConsoleExtension</c> provides extension methods for
    /// simple printing of colored console messages.
    /// </summary>
    public static class ConsoleExtension
    {
        private static Mutex mutex = new();

        /// <summary>
        /// Prints a message line using given colors on standard console.
        /// </summary>
        /// <param name="text">Line text.</param>
        /// <param name="fgColor">Text color.</param>
        /// <param name="bgColor">Background color for the text.</param>
        public static void WriteLine(
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
        public static void Write(
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
}
