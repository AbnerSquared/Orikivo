using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Framework
{
    public class LogService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public LogService(DiscordSocketClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;

            _client.Log += LogAsync;
            _commandService.Log += LogAsync;

            Logger.Debug("Initialized instance of LogService.");
        }

        /// <summary>
        /// The <see cref="Orikivo.LogConfig"/> to set for the current <see cref="LogService"/>.
        /// </summary>
        public LogConfig LogConfig { set => SetLogConfig(value); }

        public ConsoleLayout Layout => ConsoleLayout.GetCurrent();

        private LogAliasMap Aliases { get; set; }

        private LogColorMap<LogSeverity> Colors { get; set; }

        private string EntryFormat { get; set; }

        private string ExceptionFormat { get; set; }

        private string MessageFormat { get; set; }

        private string ExitFormat { get; set; }

        private void SetLogConfig(LogConfig config)
        {
            if (config == null)
                return;

            EntryFormat = config.EntryFormatting;
            ExceptionFormat = config.ExceptionFormatting;
            MessageFormat = config.MessageFormatting;
            ExitFormat = config.ExitFormatting;
            Aliases = config.Aliases;
            Colors = config.Colors;
        }

        /// <summary>
        /// Writes a <see cref="LogMessage"/> to the <see cref="Console"/> as an asynchronous method.
        /// </summary>
        public Task LogAsync(LogMessage log)
        {
            if (Colors?[log.Severity] != null && Colors[log.Severity] != (Layout.BackgroundColor, Layout.ForegroundColor))
                Logger.SetTemporaryColors(Colors[log.Severity]?.BackgroundColor, Colors[log.Severity]?.TextColor);

            var value = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(EntryFormat))
                value.AppendLine(EntryFormat
                    .Replace(Aliases[LogAlias.Name], _client.CurrentUser.Username)
                    .Replace(Aliases[LogAlias.Date], DateTime.UtcNow.ToString())
                    .Replace(Aliases[LogAlias.LogSeverity], log.Severity.ToString())
                    .Replace(Aliases[LogAlias.LogSource], log.Source));

            if (!string.IsNullOrWhiteSpace(ExceptionFormat) && log.Exception != null)
                value.AppendLine(ExceptionFormat
                    .Replace(Aliases[LogAlias.Exception], log.Exception?.ToString())
                    .Replace(Aliases[LogAlias.ExceptionType], log.Exception?.GetType().Name)
                    .Replace(Aliases[LogAlias.ExceptionMessage], log.Exception?.Message));

            if (!string.IsNullOrWhiteSpace(MessageFormat) && !string.IsNullOrWhiteSpace(log.Message))
                value.AppendLine(MessageFormat
                    .Replace(Aliases[LogAlias.LogMessage], log.Message));

            if (!string.IsNullOrWhiteSpace(ExitFormat))
                value.Append(ExitFormat);

            return Task.Run(async () =>
            {
                await Console.Out.WriteLineAsync(value.ToString());
                await Logger.WriteToFileAsync(value.ToString());
                Logger.RestoreColors();
            });
        }
    }
}
