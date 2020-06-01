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
            ReservedChannels = new Dictionary<ulong, string>();
            ReservedUsers = new Dictionary<ulong, string>();

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

        // this starts a new base game server
        public async Task CreateServerAsync(IUser user, IMessageChannel channel)
        {
            if (user.IsBot)
                return;

            if (ReservedUsers.ContainsKey(user.Id) || ReservedChannels.ContainsKey(channel.Id))
                throw new Exception("Cannot initialize a new server as either the user or a channel is reserved to an existing server");

            Player host = new Player { Host = true, User = user, JoinedAt = DateTime.UtcNow, Playing = false };

            var server = new GameServer();

            ReservedUsers.Add(user.Id, server.Id);
            ReservedChannels.Add(channel.Id, server.Id);

            var internalMessage = await channel.SendMessageAsync(server.GetDisplayChannel(0).ToString());

            ServerConnection connection = new ServerConnection
            {
                InternalChannel = channel,
                ChannelId = channel.Id,
                Frequency = 0,
                Spectator = false,
                InternalMessage = internalMessage,
                MessageId = internalMessage.Id
            };

            server.Players.Add(host);
            server.Connections.Add(connection);

            // refreshes all displays, just in case
            await server.UpdateAsync();

            Servers.Add(server.Id, server);
        }

        // destroys the game server accordingly
        internal async Task DestroyServerAsync(GameServer server)
        {
            server.GetDisplayChannel(0).Content.Override = "This server has been destroyed. Sorry about the inconvenience.";

            // update the display to notify that the server was destroyed
            foreach (ServerConnection connection in server.Connections)
            {
                connection.Frequency = 0;
            }

            await server.UpdateAsync();

            foreach (ulong channelId in server.Connections.Select(x => x.ChannelId))
                if (ReservedChannels.ContainsKey(channelId))
                    ReservedChannels.Remove(channelId);

            foreach (ulong userId in server.Players.Select(x => x.User.Id))
                if (ReservedUsers.ContainsKey(userId))
                    ReservedUsers.Remove(userId);
            
            // release all channels that were reserved to this server
        }

        // this releases a player from the reserves, and removes them from the server they are in
        internal async Task RemovePlayerAsync(Player player)
        {
            if (ReservedUsers.ContainsKey(player.User.Id))
            {
                // get the server from the key given by the reserved users
                GameServer server = Servers[ReservedUsers[player.User.Id]];

                if (server == null)
                    return;

                if (server.GetPlayer(player.User.Id) != null)
                {
                    server.Players.Remove(player);
                    ReservedUsers.Remove(player.User.Id);

                    IDMChannel dm = await player.User.GetOrCreateDMChannelAsync();
                    await dm.SendMessageAsync("You were removed from the server\nReason: Manual kick, unknown");
                }
            }
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

                        if (ReservedUsers.ContainsKey(player.User.Id))
                            ReservedUsers.Remove(player.User.Id);
                        
                        IDMChannel dm = await player.User.GetOrCreateDMChannelAsync();
                        await dm.SendMessageAsync("You were kicked from the server\nReason: No available channel to participate in");
                    }
                }

                // if there are no players, destroy the server
                if (server.Players.Count == 0)
                {
                    await DestroyServerAsync(server);
                    return;
                }

                // if there are no current hosts, set a new one by the time they joined the server
                if (server.Host == null)
                {
                    server.Players.OrderBy(x => x.JoinedAt).First().Host = true;
                }
            }

            // if everyone lost a connection to the server, return the game to the lobby and end the active game session
            // and state that a channel was destroyed, causing too many players to leave
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // ignore all bots
            if (reaction.User.GetValueOrDefault()?.IsBot ?? false)
                return;

            // check to see where this reaction was applied
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                // if the user is currently in a server, check to see if they are in the same server
                // otherwise, ignore them because they are meant for another game
                if (ReservedUsers.ContainsKey(reaction.UserId))
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

                foreach (IInput input in display.Inputs)
                {
                    InputResult result = input.TryParse(new Input { Reaction = reaction.Emote, Flag = ReactionFlag.Add });

                    if (result.IsSuccess)
                    {
                        if (result.Input.Criterion?.Invoke(player, connection, server) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(player, connection, server);

                            // if there wasn't a player before and they were added, add them to the reserves
                            if (player == null)
                            {
                                if (server.GetPlayer(reaction.UserId) != null)
                                {
                                    ReservedUsers[reaction.UserId] = server.Id;
                                }
                            }
                            else // likewise, if there was a player and they were removed, remove them from the reserves
                            {
                                if (server.GetPlayer(reaction.UserId) == null)
                                {
                                    if (ReservedUsers.ContainsKey(reaction.UserId))
                                        ReservedUsers.Remove(reaction.UserId);

                                    // if the player that was removed was the last player, destroy the server
                                    if (server.Players.Count == 0)
                                        await DestroyServerAsync(server);

                                    // if the player that was removed was the host, set a new host
                                    if (server.Host == null)
                                    {
                                        server.Players.OrderBy(x => x.JoinedAt).First().Host = true;
                                    }
                                }
                            }
                        }

                        // end the cycling of inputs when once is successful
                        return;
                    }
                }
            }
        }

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // ignore all bots
            if (reaction.User.GetValueOrDefault()?.IsBot ?? false)
                return;

            // check to see where this reaction was applied
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                // if the user is currently in a server, check to see if they are in the same server
                // otherwise, ignore them because they are meant for another game
                if (ReservedUsers.ContainsKey(reaction.UserId))
                    if (ReservedChannels[channel.Id] != ReservedUsers[reaction.UserId])
                        return;

                GameServer server = Servers[ReservedChannels[channel.Id]];

                // the player shouldn't be forced because there might be a command relating them to join or something
                Player player = server.GetPlayer(reaction.UserId);

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == channel.Id);

                // check if the reaction was appended to the right message
                if (connection.MessageId != reaction.MessageId)
                    return;

                DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                foreach (IInput input in display.Inputs)
                {
                    InputResult result = input.TryParse(new Input { Reaction = reaction.Emote, Flag = ReactionFlag.Remove });

                    if (result.IsSuccess)
                    {
                        if (result.Input.Criterion?.Invoke(player, connection, server) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(player, connection, server);

                            // if there wasn't a player before and they were added, add them to the reserves
                            if (player == null)
                            {
                                if (server.GetPlayer(reaction.UserId) != null)
                                {
                                    ReservedUsers[reaction.UserId] = server.Id;
                                }
                            }
                            else // likewise, if there was a player and they were removed, remove them from the reserves
                            {
                                if (server.GetPlayer(reaction.UserId) == null)
                                {
                                    if (ReservedUsers.ContainsKey(reaction.UserId))
                                        ReservedUsers.Remove(reaction.UserId);

                                    // if the player that was removed was the last player, destroy the server
                                    if (server.Players.Count == 0)
                                        await DestroyServerAsync(server);

                                    // if the player that was removed was the host, set a new host
                                    if (server.Host == null)
                                    {
                                        server.Players.OrderBy(x => x.JoinedAt).First().Host = true;
                                    }
                                }
                            }
                        }

                        // end the cycling of inputs when once is successful
                        return;
                    }
                }
            }
        }

        public async Task OnMessageReceived(SocketMessage message)
        {
            // ignore all bots
            if (message.Author.IsBot)
                return;

            // if this message was sent in a channel that contains a server and the user is in a server
            if (ReservedChannels.ContainsKey(message.Channel.Id))
            {
                // if the user is currently in a server, check to see if they are in the same server
                // otherwise, ignore them because they are meant for another game
                if (ReservedUsers.ContainsKey(message.Author.Id))
                    if (ReservedChannels[message.Channel.Id] != ReservedUsers[message.Author.Id])
                        return;

                GameServer server = Servers[ReservedChannels[message.Channel.Id]];
                Player player = server.GetPlayer(message.Author.Id);

                if (player == null)
                    return;

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == message.Channel.Id);

                DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                foreach (IInput input in display.Inputs)
                {
                    InputResult result = input.TryParse(message.Content);
                    if (result.IsSuccess)
                    {
                        if (result.Input.Criterion?.Invoke(player, connection, server) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(player, connection, server);

                            // if there wasn't a player before and they were added, add them to the reserves
                            if (player == null)
                            {
                                if (server.GetPlayer(message.Author.Id) != null)
                                {
                                    ReservedUsers[message.Author.Id] = server.Id;
                                }
                            }
                            else // likewise, if there was a player and they were removed, remove them from the reserves
                            {
                                if (server.GetPlayer(message.Author.Id) == null)
                                {
                                    if (ReservedUsers.ContainsKey(message.Author.Id))
                                        ReservedUsers.Remove(message.Author.Id);

                                    // if the player that was removed was the last player, destroy the server
                                    if (server.Players.Count == 0)
                                        await DestroyServerAsync(server);

                                    // if the player that was removed was the host, set a new host
                                    if (server.Host == null)
                                    {
                                        server.Players.OrderBy(x => x.JoinedAt).First().Host = true;
                                    }
                                }
                            }
                        }

                        // end the cycling of inputs when one is successful
                        return;
                    }
                }
            }
        }

        public async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            // check to see where the message was deleted

            // if the deletion was in a reserved channel, get the server that contains the channel
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                GameServer server = Servers[ReservedChannels[channel.Id]];
                ServerConnection connection = server
                    .Connections
                    .First(x => x.MessageId == message.Id);
                
                connection.InternalMessage = await connection.InternalChannel
                    .SendMessageAsync(server
                    .GetDisplayChannel(connection.Frequency).ToString());

                connection.MessageId = connection.InternalMessage.Id;
            }
        }
    }
}
