using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // this handles how everything is displayed on the console.
    public class OriConsoleService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        public OriConsoleService(DiscordSocketClient client, CommandService commandService,
            OriConsoleConfig consoleConfig = null, OriLogConfig loggerConfig = null)
        {
            Console.WriteLine("-- Initializing logging services. --");
            _client = client;
            _commandService = commandService;
            SetConsoleConfig(consoleConfig);
            SetLoggerConfig(loggerConfig ?? OriLogConfig.Default);
            _isUsingTempColors = false;
            // setting up loggers
            _client.Log += LogAsync;
            _commandService.Log += LogAsync;
            
        }
        
        public OriConsoleConfig ConsoleConfig { set { SetConsoleConfig(value); Console.WriteLine("-- Console configurations set. --"); } }
        public OriLogConfig LoggerConfig { set { SetLoggerConfig(value); Console.WriteLine("-- Logging configurations set. --"); } }

        private void SetConsoleConfig(OriConsoleConfig consoleConfig)
        {
            if (consoleConfig != null)
            {
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforeColor --");
                if (consoleConfig.TextColor.HasValue)
                    TextColor = consoleConfig.TextColor.Value;
                if (consoleConfig.BackgroundColor.HasValue)
                    BackgroundColor = consoleConfig.BackgroundColor.Value;
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforePosition --");
                if (consoleConfig.WindowPosition.HasValue)
                    WindowPosition = consoleConfig.WindowPosition.Value;
                if (consoleConfig.WindowSize.HasValue)
                    WindowSize = consoleConfig.WindowSize.Value;
                Console.WriteLine("[Debug] -- ConsoleConfig.BeforeOutput --");
                CanDebug = consoleConfig.Debug;
                OutputPath = consoleConfig.OutputPath;
                Title = consoleConfig.Title;
                Console.WriteLine("[Debug] -- ConsoleConfig.Success --");
                Console.CursorVisible = consoleConfig.ShowCursor;
            }

            Console.WriteLine("[Debug] -- ConsoleConfig.End --");
        }

        private void SetLoggerConfig(OriLogConfig loggerConfig)
        {
            if (loggerConfig != null)
            {
                EntryFormat = loggerConfig.EntryFormat;
                ExceptionFormat = loggerConfig.ExceptionFormat;
                MessageFormat = loggerConfig.MessageFormat;
                ExitFormat = loggerConfig.ExitFormat;
                LogKeys = loggerConfig.Keys;
                Colors = loggerConfig.Colors;
            }
        }

        public ConsoleColor? TextColor
        {
            get => Console.ForegroundColor;
            set
            {
                if (value.HasValue)
                    Console.ForegroundColor = value.Value;
                else
                {
                    ConsoleColor? backgroundColor = BackgroundColor;
                    Console.ResetColor();
                    if (backgroundColor.HasValue)
                        BackgroundColor = backgroundColor;
                }
            }
        }

        public ConsoleColor? BackgroundColor
        {
            get => Console.BackgroundColor;
            set
            {
                if (value.HasValue)
                    Console.BackgroundColor = value.Value;
                else
                {
                    ConsoleColor? textColor = TextColor;
                    Console.ResetColor();
                    if (textColor.HasValue)
                        TextColor = textColor;
                }
            }
        }

        public string Title
        {
            get => Console.Title;
            private set
            {
                Debug("ConsoleConfig.BeforeTitle");
                if (!string.IsNullOrWhiteSpace(value))
                    Console.Title = value;
                Debug("ConsoleConfig.AfterTitle");
            }
        }

        public Point WindowPosition
        {
            get => new Point(Console.WindowLeft, Console.WindowTop);
            private set => Console.SetWindowPosition(value.X, value.Y);
        }

        public Size WindowSize
        {
            get => new Size(Console.WindowWidth, Console.WindowHeight);
            private set => Console.SetWindowSize(value.Width, value.Height);
        }

        // TODO: Create output file that can store all logs.
        public string OutputPath { get; private set; }
        private bool CanOutput => Checks.NotNull(OutputPath);
        public bool CanDebug { get; set; }
        private LogKeyMap LogKeys { get; set; }
        private LogColorMap Colors { get; set; }
        private string EntryFormat { get; set; }
        private string ExceptionFormat { get; set; }
        private string MessageFormat { get; set; }
        private string ExitFormat { get; set; }

        public void WriteLine(string message, ConsoleColor? textColor = null, ConsoleColor? backgroundColor = null)
        {
            SetTempColors(backgroundColor, textColor);
            Console.WriteLine(message);
            RemoveTempColors();
        }

        public void Debug(string content)
        {
            if (CanDebug)
                WriteLine(string.Format(OriFormat.DebugFormat, content));
        }

        private ConsoleColor? _oldBackgroundColor;
        private ConsoleColor? _oldTextColor;
        private bool _isUsingTempColors;
        private void SetTempColors(ConsoleColor? backgroundColor = null, ConsoleColor? textColor = null)
        {
            if (!_isUsingTempColors)
            {
                _isUsingTempColors = true;
                _oldBackgroundColor = BackgroundColor;
                _oldTextColor = TextColor;
                BackgroundColor = backgroundColor;
                TextColor = textColor;
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
                BackgroundColor = _oldBackgroundColor;
                TextColor = _oldTextColor;
                _oldBackgroundColor = null;
                _oldTextColor = null;
                //Console.WriteLine("[Debug] -- Previous color values returned. --");
            }
            else
                Console.WriteLine("[Debug] -- Temporary colors are not in use. --");
        }

        private string GetKey(LogKey keyType)
            => LogKeys[keyType];

        public Task LogAsync(LogMessage log)
        {
            // TODO: could be able to remove Colors[log.Severity] != null
            if (Colors[log.Severity] != null && Colors?[log.Severity] != (BackgroundColor, TextColor))
                SetTempColors(Colors?[log.Severity]?.BackgroundColor, Colors?[log.Severity]?.TextColor);

            StringBuilder sb = new StringBuilder();
            // $"Orikivo [{DateTime.UtcNow}]\n[{log.Severity}.{log.Source}]"
            sb.AppendLine(EntryFormat
                .Replace(GetKey(LogKey.Name), "Orikivo")
                .Replace(GetKey(LogKey.Date), DateTime.UtcNow.ToString())
                .Replace(GetKey(LogKey.LogSeverity), log.Severity.ToString())
                .Replace(GetKey(LogKey.LogSource), log.Source)
                );

            sb.AppendLine(log.Exception?.ToString() ?? log.Message);

            if (Checks.NotNull(ExitFormat))
                sb.Append(ExitFormat);

            return Task.Run(async () => { await Console.Out.WriteLineAsync(sb.ToString()); RemoveTempColors(); });
        }
    }
}
