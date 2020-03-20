using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// The event handler deriving from all events relating to the Discord API.
    /// </summary>
    public class EventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly OriJsonContainer _container;

        public EventHandler(DiscordSocketClient client, OriJsonContainer container)
        {
            Console.WriteLine("-- Initializing event handler. --");
            _client = client;
            _container = container;
            _client.Ready += OnReadyAsync;
            _client.UserJoined += OnUserJoinAsync;
            //_client.ChannelDestroyed += OnChannelDeletedAsync;
        }

        private async Task OnUserJoinAsync(SocketGuildUser user)
        {
            OriGuild server = _container.GetOrAddGuild(user.Guild);
            
            if (server.Options.DefaultRoleId.HasValue)
                await user.AddRoleAsync(user.Guild.GetRole(server.Options.DefaultRoleId.Value));
            
            if (server.Options.UseEvents)
            {
                SocketTextChannel defaultChannel = user.Guild.SystemChannel;

                if (server.Options.SystemChannelId.HasValue)
                    defaultChannel = user.Guild.GetTextChannel(server.Options.SystemChannelId.Value) ?? defaultChannel;

                if (defaultChannel != null && server.Options.Greetings.Count > 0)
                    await defaultChannel.SendMessageAsync(server.Greet(user.Guild, user));
            }
        }

        /*
        private async Task OnChannelDeletedAsync(SocketChannel channel)
        {
            Game game = _games.Games.Values.FirstOrDefault(x => x.Receivers.Any(y => y.ChannelId == channel.Id));
            if (game.Receivers.Count - 1 == 0) // if the receiver count - the one about to be deleted results in an empty game.
                await _games.DeleteGameAsync(game.Id);

        }
        */

        private async Task OnReadyAsync()
        {
            //_logger.Debug("Orikivo has connected to Discord.");
        }
    }
}
