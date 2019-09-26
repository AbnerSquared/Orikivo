using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Text;

namespace Orikivo.Logging
{
    public class Logger
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public Logger(DiscordSocketClient client, CommandService service)
        {
            _client = client;
            _service = service;

            //_client.Log += LogAsync;
            //_service.Log += LogAsync;

        }

        private static Task LogAsync(LogMessage m)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine($"Orikivo [{DateTime.UtcNow}]");
            log.AppendLine($"{m.Severity}.{m.Source}");
            log.AppendLine(m.Exception?.ToString() ?? m.Message);
            return Console.Out.WriteLineAsync(log.ToString());
        }

        public static void Log(string author, string task, string message = null)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine($"{author} [{DateTime.UtcNow}]");
            log.AppendLine($"{task}");
            if (!string.IsNullOrWhiteSpace(message))
            {
                log.AppendLine(message);
            }
            Console.WriteLine(log.ToString());
        }

        // neatly writes an exception.
        public static void WriteException(Exception ex)
        {

        }
        
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}