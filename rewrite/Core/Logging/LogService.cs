using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// A service that controls all log-related events.
    /// </summary>
    public class LogService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        /// <summary>
        /// Creates a new <see cref="LogService"/> with the specified parameters.
        /// </summary>
        /// <param name="client">The <see cref="BaseSocketClient"/> to link with the new <see cref="LogService"/>.</param>
        /// <param name="commandService">The <see cref="CommandService"/> to link with the new <see cref="LogService"/>.</param>
        /// <param name="consoleConfig">The configuration used to build the console-side values for the new <see cref="LogService"/>.</param>
        /// <param name="logConfig">The configuration used to handle event-side values for the new <see cref="LogService"/>.</param>
        public LogService(DiscordSocketClient client, CommandService commandService,
            ConsoleConfig consoleConfig = null, LogConfig logConfig = null)
        {
            _client = client;
            _commandService = commandService;

            SetLogConfig(logConfig ?? LogConfig.Default);
            SetConsoleConfig(consoleConfig);

            _client.Log += LogAsync;
            _commandService.Log += LogAsync;

            Debug("Initialized console logger.");
        }
        
        /// <summary>
        /// The <see cref="Orikivo.ConsoleConfig"/> to set for the current <see cref="LogService"/>.
        /// </summary>
        public ConsoleConfig ConsoleConfig { set => SetConsoleConfig(value); }

        /// <summary>
        /// The <see cref="Orikivo.LogConfig"/> to set for the current <see cref="LogService"/>.
        /// </summary>
        public LogConfig LogConfig { set => SetLogConfig(value); }

        private bool _isUsingTempColors = false;

        private ConsoleColor? _tempTextColor;

        /// <summary>
        /// The current <see cref="Console.ForegroundColor"/>.
        /// </summary>
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

        private ConsoleColor? _tempBackgroundColor;

        /// <summary>
        /// The current <see cref="Console.BackgroundColor"/>.
        /// </summary>
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

        /// <summary>
        /// The current <see cref="Console.Title"/>.
        /// </summary>
        public string Title
        {
            get => Console.Title;
            private set
            {

                if (Checks.NotNull(value))
                    Console.Title = value;

                Debug("ConsoleConfig.SetTitle");
            }
        }

        /// <summary>
        /// The current position of the <see cref="Console"/> window.
        /// </summary>
        public Point WindowPosition
        {
            get => new Point(Console.WindowLeft, Console.WindowTop);
            private set => Console.SetWindowPosition(value.X, value.Y);
        }

        /// <summary>
        /// The current window <see cref="Size"/> of the <see cref="Console"/>.
        /// </summary>
        public Size WindowSize
        {
            get => new Size(Console.WindowWidth, Console.WindowHeight);
            private set => Console.SetWindowSize(value.Width, value.Height);
        }

        // TODO: Create output file that can store all logs.
        /// <summary>
        /// The directory that points to where logs are stored.
        /// </summary>
        public string OutputPath { get; private set; }

        /// <summary>
        /// A <see cref="bool"/> value that states if debugging is logged to the <see cref="Console"/>.
        /// </summary>
        public bool CanDebug { get; set; }

        private LogAliasMap Aliases { get; set; }

        private LogColorMap<LogSeverity> Colors { get; set; }

        private string EntryFormat { get; set; }

        private string ExceptionFormat { get; set; }

        private string MessageFormat { get; set; }

        private string ExitFormat { get; set; }

        private void SetConsoleConfig(ConsoleConfig config)
        {
            if (config != null)
            {
                if (config.TextColor.HasValue)
                    TextColor = config.TextColor.Value;

                if (config.BackgroundColor.HasValue)
                    BackgroundColor = config.BackgroundColor.Value;

                Debug("ConsoleConfig.SetColorPair");

                if (config.Position.HasValue)
                    WindowPosition = config.Position.Value;

                if (config.Size.HasValue)
                    WindowSize = config.Size.Value;

                Debug("ConsoleConfig.SetWindow");

                CanDebug = config.Debug;
                OutputPath = config.OutputPath;
                Title = config.Title;
                Console.CursorVisible = config.ShowCursor;
            }

            Console.WriteLine("ConsoleConfig.End");
        }

        private void SetLogConfig(LogConfig config)
        {
            if (config != null)
            {
                EntryFormat = config.EntryFormat;
                ExceptionFormat = config.ExceptionFormat;
                MessageFormat = config.MessageFormat;
                ExitFormat = config.ExitFormat;
                Aliases = config.Aliases;
                Colors = config.Colors;
            }
        }

        private void SetTempColors(ConsoleColor? backgroundColor = null, ConsoleColor? textColor = null)
        {
            if (!_isUsingTempColors)
            {
                _isUsingTempColors = true;
                _tempBackgroundColor = BackgroundColor;
                _tempTextColor = TextColor;
                BackgroundColor = backgroundColor;
                TextColor = textColor;
            }
            else
                Console.WriteLine(string.Format(OriFormat.DebugFormat, "Temporary colors have already been set."));
        }

        private void RestoreColors()
        {
            if (_isUsingTempColors)
            {
                _isUsingTempColors = false;
                BackgroundColor = _tempBackgroundColor;
                TextColor = _tempTextColor;
                _tempBackgroundColor = null;
                _tempTextColor = null;
            }
        }

        /// <summary>
        /// Wrtites a line to the <see cref="Console"/>, only if <see cref="LogService.CanDebug"/> is enabled.
        /// </summary>
        public void Debug(string content)
        {
            if (CanDebug)
            {
                WriteLine(string.Format(OriFormat.DebugFormat, content));

                if (Checks.NotNull(OutputPath))
                    WriteToFileAsync(string.Format(OriFormat.DebugFormat, content));
            }
            
        }

        /// <summary>
        /// Writes a line to the <see cref="Console"/> with an optional pair of <see cref="ConsoleColor"/> values.
        /// </summary>
        public void WriteLine(string message, ConsoleColor? textColor = null, ConsoleColor? backgroundColor = null)
        {
            if (textColor == null && backgroundColor == null)
                Console.WriteLine(message);
            else
            {
                SetTempColors(backgroundColor, textColor);
                Console.WriteLine(message);
                RestoreColors();
            }

            if (Checks.NotNull(OutputPath))
                WriteToFileAsync(message);
        }

        /// <summary>
        /// Writes a <see cref="LogMessage"/> to the <see cref="Console"/> as an asynchronous method.
        /// </summary>
        public Task LogAsync(LogMessage log)
        {
            if (Checks.NotNull(Colors?[log.Severity]) && Colors[log.Severity] != (BackgroundColor, TextColor))
                SetTempColors(Colors[log.Severity]?.BackgroundColor, Colors[log.Severity]?.TextColor);

            StringBuilder value = new StringBuilder();

            if (Checks.NotNull(EntryFormat))
                value.AppendLine(EntryFormat
                    .Replace(Aliases[LogAlias.Name], OriGlobal.ClientName)
                    .Replace(Aliases[LogAlias.ClientVersion], OriGlobal.ClientVersion)
                    .Replace(Aliases[LogAlias.Date], DateTime.UtcNow.ToString())
                    .Replace(Aliases[LogAlias.LogSeverity], log.Severity.ToString())
                    .Replace(Aliases[LogAlias.LogSource], log.Source));

            if (Checks.NotNull(ExceptionFormat) && Checks.NotNull(log.Exception))
                value.AppendLine(ExceptionFormat
                    .Replace(Aliases[LogAlias.Exception], log.Exception?.ToString())
                    .Replace(Aliases[LogAlias.ExceptionType], log.Exception?.GetType().Name)
                    .Replace(Aliases[LogAlias.ExceptionMessage], log.Exception?.Message));

            if (Checks.NotNull(MessageFormat) && Checks.NotNull(log.Message))
                value.AppendLine(MessageFormat
                    .Replace(Aliases[LogAlias.LogMessage], log.Message));

            if (Checks.NotNull(ExitFormat))
                value.Append(ExitFormat);

            return Task.Run(async () =>
            {
                await Console.Out.WriteLineAsync(value.ToString());

                if (Checks.NotNull(OutputPath))
                    await WriteToFileAsync(value.ToString());

                RestoreColors();
            });
        }

        private async Task WriteToFileAsync(string content)
        {
            if (Checks.NotNull(OutputPath))
            {
                string directory = Directory.CreateDirectory(OutputPath).FullName;
                string path = $"{directory}{DateTime.UtcNow.ToString("MM-dd-yyyy")}_log.txt";
                // TODO: Implement logging
                using (StreamWriter writer = File.AppendText(path))
                    await writer.WriteLineAsync(content);
            }
        }
    }
}
