using Discord.WebSocket;
using System.Threading.Tasks;
using Orikivo.Framework;

namespace Orikivo
{
    /// <summary>
    /// The event handler deriving from all events relating to the Discord API.
    /// </summary>
    public class EventHandler
    {
        private readonly DiscordSocketClient _client;

        public EventHandler(DiscordSocketClient client)
        {
            Logger.Debug("-- Initializing event handler. --");
            _client = client;
            _client.Ready += OnReadyAsync;
        }
        private async Task OnReadyAsync()
        {
            //_logger.Debug("Orikivo has connected to Discord.");
        }
    }
}
