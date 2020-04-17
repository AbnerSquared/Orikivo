using System;
using System.IO;
using System.Text;

namespace Orikivo.Framework
{
    public static class Logger
    {
        public const ConsoleColor DEFAULT_FOREGROUND_COLOR = ConsoleColor.Gray;
        public const ConsoleColor DEFAULT_BACKGROUND_COLOR = ConsoleColor.Black;

        private static ConsoleColor? _foregroundColor;
        private static ConsoleColor? _backgroundColor;

        public static string LogDirectory { get; set; }

        public static bool DebugAllowed { get; set; }

        public static ConsoleColor? ForegroundColor
        {
            get
            {
                return Console.ForegroundColor;
            }
            set
            {
                Console.ForegroundColor = value ?? DEFAULT_FOREGROUND_COLOR;
            }
        }

        public static ConsoleColor? BackgroundColor
        {
            get
            {
                return Console.BackgroundColor;
            }
            set
            {
                Console.BackgroundColor = value ?? DEFAULT_BACKGROUND_COLOR;
            }
        }

        public static void Log(ConsoleString value)
        {
            WriteLine(value);
            WriteToFile(value.Content);
        }

        public static void Log(string content)
        {
            WriteLine(content);
            WriteToFile(content);
        }

        public static void Debug(ConsoleString value)
        {
            if (DebugAllowed)
            {
                WriteLine(value);
            }
        }

        public static void Debug(string content)
        {
            if (DebugAllowed)
            {
                WriteLine(content);
            }
        }

        public static void WriteLine(ConsoleString value)
        {
            SetTemporaryColors(value.Color, value.Highlight);
            WriteLine(value.Content);
            RestoreColors();
        }

        public static void WriteLine(string content)
            => Console.WriteLine(content);

        private static void SetTemporaryColors(ConsoleColor? foreground, ConsoleColor? background)
        {
            _foregroundColor = ForegroundColor;
            _backgroundColor = BackgroundColor;
            ForegroundColor = foreground;
            BackgroundColor = background;
        }

        private static void RestoreColors()
        {
            ForegroundColor = _foregroundColor;
            BackgroundColor = _backgroundColor;
            _foregroundColor = null;
            _backgroundColor = null;
        }

        private static void WriteToFile(string content)
        {
            if (string.IsNullOrWhiteSpace(LogDirectory))
                return;

            var path = Directory.CreateDirectory($"../logs/{LogDirectory}").FullName + GetLogFileName();

            using (StreamWriter writer = File.AppendText(path))
            {
                writer.WriteLine(content);
            }
        }

        private static string GetLogFileName()
        {
            var file = new StringBuilder();

            file.Append(DateTime.UtcNow.ToString("MM_dd_yyyy"));
            file.Append("_LOG.txt");

            return file.ToString();
        }
    }
}
