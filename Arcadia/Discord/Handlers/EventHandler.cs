using Discord.WebSocket;
using System.Threading.Tasks;
using Orikivo.Framework;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia
{
    /// <summary>
    /// The event handler deriving from all events relating to the Discord API.
    /// </summary>
    public class EventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly ArcadeContainer _container;

        public EventHandler(DiscordSocketClient client, ArcadeContainer container)
        {
            _container = container;
            Logger.Debug("-- Initializing event handler. --");
            _client = client;
            _client.Ready += OnReadyAsync;
            _client.LatencyUpdated += TryResetMonthYear;
            
        }

        private async Task OnReadyAsync()
        {
            Logger.Debug($"{_client.CurrentUser.Username} is ready.");
        }

        private async Task TryResetMonthYear(int previous, int current)
        {
            long currentMonthYear = _container.Data.GetMonthYear();

            if (_container.Data.CurrentMonthYear != currentMonthYear)
                _container.Data.CurrentMonthYear = currentMonthYear; // await StartNewMonthYear(currentMonthYear);
        }
    }
}
