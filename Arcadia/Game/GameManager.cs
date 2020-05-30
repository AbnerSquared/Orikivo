using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arcadia

{
    public class GameManager
    {
        private readonly DiscordSocketClient _client;

        public GameManager(DiscordSocketClient client)
        {
            _client = client;
            Servers = new Dictionary<string, GameServer>();

            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            _client.MessageReceived += OnMessageReceived;
            _client.MessageDeleted += OnMessageDeleted;
        }

        internal Dictionary<string, GameServer> Servers { get; set; }

        // this is all channels that are currently bound to a game server
        internal Dictionary<ulong, string> ReservedChannels { get; set; }

        internal Dictionary<ulong, string> ReservedUsers { get; set; }

        // destroys the game server accordingly
        internal void DestroyServer(GameServer server)
        {
            // update the display to notify that the server was destroyed
            
            // release all channels that were reserved to this server
        }

        public async Task OnChannelDestroyed(SocketChannel channel)
        {
            // if a channel is destroyed, check for everyone that could only see that channel

            // if everyone lost a connection to the server, return the game to the lobby and end the active game session
            // and state that a channel was destroyed, causing too many players to leave
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // check to see where this reaction was applied

            // if the reaction was in a reserved channel, get the server that contains that channel
            
            // search to see if there were any available input reactions that matches the reaction used

                // if there was a input found, check to see if the flag allows this reaction method

                    // if the reaction method was allowed, invoke
        }

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // check to see where this reaction was applied

            // if the reaction was in a reserved channel, get the server that contains that channel

            // search to see if there were any available input reactions that matches the reaction used

            // if there was an input found, check to see if the flag allows this reaction method

            // if the reaction method was allowed, invoke
        }

        public async Task OnMessageReceived(SocketMessage message)
        {
            // check to see where this message was sent

            // if the reaction was in a reserved channel, get the server that contains the channel

            // search to see if there were any available inputs that matches the text used

            // if there was an input found, invoke
        }

        public async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            // check to see where the message was deleted

            // if the deletion was in a reserved channel, get the server that contains the channel

            // search to see if the message deleted was the main display

            // if the main display was deleted, create a new display and re-establish a new id bind
        }
    }
}
