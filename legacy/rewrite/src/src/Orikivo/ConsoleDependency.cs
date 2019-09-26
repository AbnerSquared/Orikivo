using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Orikivo.Systems.Dependencies
{
    public class ConsoleDependency
    {
        private readonly DiscordSocketClient _socket;
        private readonly CommandService _service;

        public ConsoleDependency(DiscordSocketClient socket, CommandService service)
        {
            _socket = socket;
            _service = service;

            _socket.Log += OnConsoleAsync;
            _service.Log += OnConsoleAsync;
        }

        private static Task OnConsoleAsync(LogMessage m)
        {
            //string logBase = $"Orikivo [{DateTime.UtcNow}]\n{message.Severity.ToString()}.{message.Source}\n{message.Exception?.ToString() ?? message.Message.ToLower()}\n";
            StringBuilder log = new StringBuilder();
            log.AppendLine($"Orikivo [{DateTime.UtcNow}]");
            log.AppendLine($"{m.Severity}.{m.Source}");
            log.AppendLine(m.Exception?.ToString() ?? m.Message);

            return Console.Out.WriteLineAsync(log.ToString());
        }
    }
}