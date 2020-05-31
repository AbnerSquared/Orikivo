using Discord;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                GameServer server = Servers[ReservedChannels[channel.Id]];

                ServerConnection connection = server.Connections.First(x => x.ChannelId == channel.Id);

                Dictionary<Player, List<ulong>> playerConnections = await server.GetPlayerConnectionsAsync();

                server.Connections.Remove(connection);

                foreach ((Player player, List<ulong> channelIds) in playerConnections)
                {
                    // if the channel that was deleted is the only channel a player can view, remove them from the game
                    if (channelIds.Count == 1 && channelIds.Contains(channel.Id))
                    {
                        server.Players.Remove(player);
                        IDMChannel dm = await player.User.GetOrCreateDMChannelAsync();
                        await dm.SendMessageAsync("You were kicked from the server\nReason: No available channel to participate in");
                    }
                }
            }

            // if everyone lost a connection to the server, return the game to the lobby and end the active game session
            // and state that a channel was destroyed, causing too many players to leave
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // check to see where this reaction was applied
            if (ReservedChannels.ContainsKey(channel.Id) && ReservedUsers.ContainsKey(reaction.UserId))
            {
                if (ReservedChannels[channel.Id] != ReservedUsers[reaction.UserId])
                    return;

                GameServer server = Servers[ReservedChannels[channel.Id]];
                Player player = server.GetPlayer(reaction.UserId);

                if (player == null)
                    return;

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == channel.Id);

                // check if the reaction was appended to the right message
                if (connection.MessageId != reaction.MessageId)
                    return;

                DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                foreach (ReactionInput input in display.ReactionInputs)
                {
                    if (!input.Flag.HasFlag(ReactionFlag.Add))
                        continue;

                    InputResult result = input.TryParse(reaction.Emote);

                    if (result.IsSuccess)
                    {
                        if (result.Input.Criterion?.Invoke(player) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(player, server);
                        }
                    }
                }
            }
        }

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // check to see where this reaction was applied
            if (ReservedChannels.ContainsKey(channel.Id) && ReservedUsers.ContainsKey(reaction.UserId))
            {
                if (ReservedChannels[channel.Id] != ReservedUsers[reaction.UserId])
                    return;

                GameServer server = Servers[ReservedChannels[channel.Id]];
                Player player = server.GetPlayer(reaction.UserId);

                if (player == null)
                    return;

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == channel.Id);

                // check if the reaction was appended to the right message
                if (connection.MessageId != reaction.MessageId)
                    return;

                DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                foreach (ReactionInput input in display.ReactionInputs)
                {
                    if (!input.Flag.HasFlag(ReactionFlag.Remove))
                        continue;

                    InputResult result = input.TryParse(reaction.Emote);

                    if (result.IsSuccess)
                    {
                        if (result.Input.Criterion?.Invoke(player) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(player, server);
                        }
                    }
                }
            }
        }

        public async Task OnMessageReceived(SocketMessage message)
        {
            // if this message was sent in a channel that contains a server and the user is in a server
            if (ReservedChannels.ContainsKey(message.Channel.Id) && ReservedUsers.ContainsKey(message.Author.Id))
            {
                if (ReservedChannels[message.Channel.Id] != ReservedUsers[message.Author.Id])
                    return;

                GameServer server = Servers[ReservedChannels[message.Channel.Id]];
                Player player = server.GetPlayer(message.Author.Id);

                if (player == null)
                    return;

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == message.Channel.Id);

                DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                foreach (TextInput input in display.TextInputs)
                {
                    InputResult result = input.TryParse(message.Content);
                    if (result.IsSuccess)
                    {
                        if (result.Input.Criterion?.Invoke(player) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(player, server);
                        }
                    }
                }
            }

            // if the reaction was in a reserved channel, get the server that contains the channel

            // search to see if there were any available inputs that matches the text used

            // if there was an input found, invoke
        }

        public async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            // check to see where the message was deleted

            // if the deletion was in a reserved channel, get the server that contains the channel
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                ServerConnection connection = Servers[ReservedChannels[channel.Id]]
                    .Connections
                    .First(x => x.MessageId == message.Id);
                
                connection.InternalMessage = await connection.InternalChannel
                    .SendMessageAsync(Servers[ReservedChannels[channel.Id]]
                    .GetDisplayChannel(connection.Frequency).ToString());

                connection.MessageId = connection.InternalMessage.Id;
            }

            // search to see if the message deleted was the main display

            // if the main display was deleted, create a new display and re-establish a new id bind
        }
    }
}
