using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // the class that handles all forms of logging
    // this is where the console writes lines
    public class OriLoggerService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        public OriLoggerService(DiscordSocketClient client, CommandService commandService,
            OriConsoleConfig consoleConfig = null, OriLoggerConfig loggerConfig = null)
        {
            Console.WriteLine("-- Initializing logging services. --");
            _client = client;
            _commandService = commandService;
            SetConsoleConfig(consoleConfig);
            SetLoggerConfig(loggerConfig);
            _isUsingTempColors = false;
            // setting up loggers
            _client.Log += WriteLogAsync;
            _commandService.Log += WriteLogAsync;
            
        }
        
        public OriConsoleConfig ConsoleConfig { set { SetConsoleConfig(value); Console.WriteLine("-- Console configurations set. --"); } }
        public OriLoggerConfig LoggerConfig { set { SetLoggerConfig(value); Console.WriteLine("-- Logging configurations set. --"); } }

        private void SetConsoleConfig(OriConsoleConfig consoleConfig)
        {
            if (consoleConfig != null)
            {
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforeColor --");
                if (consoleConfig.TextColor.HasValue)
                    ConsoleTextColor = consoleConfig.TextColor.Value;
                if (consoleConfig.BackgroundColor.HasValue)
                    ConsoleBackgroundColor = consoleConfig.BackgroundColor.Value;
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforePosition --");
                if (consoleConfig.WindowPosition.HasValue)
                    ConsoleWindowPosition = consoleConfig.WindowPosition.Value;
                if (consoleConfig.WindowSize.HasValue)
                    ConsoleWindowSize = consoleConfig.WindowSize.Value;
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforeOutput --");
                CanDebug = consoleConfig.Debug;
                OutputPath = consoleConfig.OutputPath;
                ConsoleTitle = consoleConfig.Title;
                Console.WriteLine("[Debug] -- ConsoleConfig.Success --");
                Console.CursorVisible = consoleConfig.ShowCursor;
            }

            Console.WriteLine("[Debug] -- ConsoleConfig.End --");
        }

        private void SetLoggerConfig(OriLoggerConfig loggerConfig)
        {
            if (loggerConfig != null)
            {
                _logKeyEntry = loggerConfig.LogKeyEntry;
                _logKeyExit = loggerConfig.LogKeyExit;
                _entryFormat = loggerConfig.EntryFormat;
                _exceptionFormat = loggerConfig.ExceptionFormat;
                _messageFormat = loggerConfig.MessageFormat;
                _exitFormat = loggerConfig.ExitFormat;
                _logKeys = loggerConfig.LogKeys;
                _logColors = loggerConfig.LogColors;
            }
        }

        public ConsoleColor? ConsoleTextColor
        {
            get { return Console.ForegroundColor; }
            set
            {
                if (value.HasValue)
                    Console.ForegroundColor = value.Value;
                else
                {
                    ConsoleColor? oldBackgroundColor = ConsoleBackgroundColor;
                    Console.ResetColor();
                    if (oldBackgroundColor.HasValue)
                        ConsoleBackgroundColor = oldBackgroundColor;
                }
            }
        }

        public ConsoleColor? ConsoleBackgroundColor
        {
            get { return Console.BackgroundColor; }
            set
            {
                if (value.HasValue)
                    Console.BackgroundColor = value.Value;
                else
                {
                    ConsoleColor? oldTextColor = ConsoleTextColor;
                    Console.ResetColor();
                    if (oldTextColor.HasValue)
                        ConsoleTextColor = oldTextColor.Value;
                }
            }
        }

        public string ConsoleTitle
        {
            get { return Console.Title; }
            private set
            {
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforeTitle --");
                if (!string.IsNullOrWhiteSpace(value))
                    Console.Title = value;
                Console.WriteLine("[Debug] -- ConsoleConfig.AfterTitle --");
            }
        }

        public Point ConsoleWindowPosition
        {
            get { return new Point(Console.WindowLeft, Console.WindowTop); }
            private set { Console.SetWindowPosition(value.X, value.Y); }
        }

        public Size ConsoleWindowSize
        {
            get { return new Size(Console.WindowWidth, Console.WindowHeight); }
            private set { Console.SetWindowSize(value.Width, value.Height); }
        }
        public string OutputPath { get; private set; }
        private bool _canOutput { get { return !string.IsNullOrWhiteSpace(OutputPath); } }
        public bool CanDebug { get; set; }
        private string _logKeyEntry { get; set; }
        private string _logKeyExit { get; set; }
        private OriLogKeyConfig _logKeys { get; set; }
        private OriLogColorConfig _logColors { get; set; }
        private string _entryFormat { get; set; }
        private string _exceptionFormat { get; set; }
        private string _messageFormat { get; set; }
        private string _exitFormat { get; set; }

        public void WriteLine(string message, ConsoleColor? textColor = null, ConsoleColor? backgroundColor = null)
        {
            SetTempColors(backgroundColor, textColor);
            Console.WriteLine(message);
            RemoveTempColors();
        }

        public void Debug(string content)
        {
            if (CanDebug)
                WriteLine(string.Format(OriFormat.DebugFrame, content));
        }

        // now format
        private ConsoleColor? _oldBackgroundColor;
        private ConsoleColor? _oldTextColor;
        private bool _isUsingTempColors;
        private void SetTempColors(ConsoleColor? backgroundColor = null, ConsoleColor? textColor = null)
        {
            if (!_isUsingTempColors)
            {
                _isUsingTempColors = true;
                _oldBackgroundColor = ConsoleBackgroundColor;
                _oldTextColor = ConsoleTextColor;
                ConsoleBackgroundColor = backgroundColor;
                ConsoleTextColor = textColor;
                //Console.WriteLine("[Debug] -- Temporary colors set. --");
            }
            else
                Console.WriteLine("[Debug] -- Temporary colors are already in use. --");
        }
        private void RemoveTempColors()
        {
            if (_isUsingTempColors)
            {
                _isUsingTempColors = false;
                ConsoleBackgroundColor = _oldBackgroundColor;
                ConsoleTextColor = _oldTextColor;
                _oldBackgroundColor = null;
                _oldTextColor = null;
                //Console.WriteLine("[Debug] -- Previous color values returned. --");
            }
            else
                Console.WriteLine("[Debug] -- Temporary colors are not in use. --");
        }

        private string GetLogKeyFormat(LogKeyType keyType)
            => $"{_logKeyEntry}{_logKeys[keyType]}{_logKeyExit}";

        public Task WriteLogAsync(LogMessage log)
        {
            if (_logColors[log.Severity] != null && _logColors?[log.Severity] != (ConsoleBackgroundColor, ConsoleTextColor))
                SetTempColors(_logColors?[log.Severity]?.BackgroundColor, _logColors?[log.Severity]?.TextColor);

            StringBuilder sb = new StringBuilder();
            // $"Orikivo [{DateTime.UtcNow}]\n[{log.Severity}.{log.Source}]"
            sb.AppendLine(_entryFormat
                .Replace(GetLogKeyFormat(LogKeyType.Name), "Orikivo")
                .Replace(GetLogKeyFormat(LogKeyType.Date), DateTime.UtcNow.ToString())
                .Replace(GetLogKeyFormat(LogKeyType.LogSeverity), log.Severity.ToString())
                .Replace(GetLogKeyFormat(LogKeyType.LogSource), log.Source)
                ); // custom entry format

            if (log.Exception != null)
                sb.AppendLine(log.Exception?.ToString()); // custom error display format
            else
                sb.AppendLine(log.Message); // custom message format

            if (!string.IsNullOrWhiteSpace(_exitFormat))
                sb.Append(_exitFormat);
            // custom exit format

            return WriteTempLineAsync(sb.ToString());//Console.Out.WriteLineAsync(sb.ToString());
        }

        private async Task WriteTempLineAsync(string value)
        {
            await Console.Out.WriteLineAsync(value);
            RemoveTempColors(); // just in case there are temp colors in use;
        }
    }
}
