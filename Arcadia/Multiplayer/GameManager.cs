using Arcadia.Games;
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

        internal static Dictionary<string, GameBuilder> Games => new Dictionary<string, GameBuilder>
        {
            ["Trivia"] = new TriviaGame()
        };

        internal Dictionary<string, GameServer> Servers { get; set; }

        // this is all channels that are currently bound to a game server
        internal Dictionary<ulong, string> ReservedChannels { get; set; }

        internal Dictionary<ulong, string> ReservedUsers { get; set; }

        public static GameBuilder GetGame(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Games[gameId];
        }

        // this attempts to join an existing server, if one is present
        public async Task JoinServerAsync(IUser user, IMessageChannel channel, string serverId)
        {
            if (user.IsBot)
                return;

            // if the user is already in a server
            if (ReservedUsers.ContainsKey(user.Id))
            {
                // and the user is in this server
                if (ReservedUsers[user.Id] == serverId)
                {
                    await channel.SendMessageAsync("You have already joined this server elsewhere.");
                    return;
                }

                throw new Exception("Cannot join this server as the user is reserved to a different server");
            }

            // if the channel this was called in is reserved
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                // and the channel is reserved to this server
                if (ReservedChannels[channel.Id] != serverId)
                {
                    throw new Exception("Cannot initialize a connection to this server as the channel is already reserved to a different server");
                }
            }

            // if the specified server doesn't exist
            if (!Servers.ContainsKey(serverId))
                throw new Exception("Cannot find a server with the specified ID");

            // otherwise, establish a new connection to this server.
            GameServer server = Servers[serverId];

            if (server.GetPlayer(user.Id) != null)
            {
                await channel.SendMessageAsync("You have already joined this server.");
                return;
            }
            
            DisplayContent content = server.GetDisplayChannel(GameState.Waiting).Content;
            DisplayContent editing = server.GetDisplayChannel(GameState.Editing).Content;

            Player player = new Player
            {
                Host = false,
                User = user,
                JoinedAt = DateTime.UtcNow,
                Playing = false
            };

            server.Players.Add(player);
            ReservedUsers.Add(user.Id, server.Id);

            content.GetComponent("header")
                                .Draw(server.Config.Title,
                                    server.Id,
                                    server.Config.GameId,
                                    server.Players.Count,
                                    "infinite players");

            (content.GetComponent("message_box") as ComponentGroup)
                    .Append($"[Console] {user.Username} has joined.");

            (editing.GetComponent("message_box") as ComponentGroup)
                .Append($"[Console] {user.Username} has joined.");

            content.GetComponent("message_box").Draw();
            editing.GetComponent("message_box").Draw(server.Config.Title);

            // if there's a connection already in place, simply update this message.
            ServerConnection connection = server.Connections.FirstOrDefault(x => x.ChannelId == channel.Id);

            // otherwise, if there isn't a connection, establish a new one.
            if (connection == null)
            {
                IUserMessage internalMessage = await channel.SendMessageAsync(content.ToString());

                connection = new ServerConnection
                {
                    ChannelId = channel.Id,
                    MessageId = internalMessage.Id,
                    InternalChannel = channel,
                    InternalMessage = internalMessage,
                    Frequency = 0,
                    State = GameState.Waiting,
                    LastRefreshed = DateTime.UtcNow
                }; // TODO: implement CanDeleteMessages

                server.Connections.Add(connection);

                
                ReservedChannels.Add(channel.Id, server.Id);
                //await channel.SendMessageAsync($"You have joined **{server.Config.Title}**.");
            }

            
            await server.UpdateAsync();
        }

        // this starts a new base game server
        public async Task CreateServerAsync(IUser user, IMessageChannel channel)
        {
            if (user.IsBot)
                return;

            if (ReservedUsers.ContainsKey(user.Id) || ReservedChannels.ContainsKey(channel.Id))
                throw new Exception("Cannot initialize a new server as either the user or a channel is reserved to an existing server");

            Player host = new Player
            {
                Host = true,
                User = user,
                JoinedAt = DateTime.UtcNow,
                Playing = false
            };

            var server = new GameServer();
            server.Config = new GameServerConfig
            {
                Privacy = Privacy.Public,
                GameId = "Trivia",
                Title = $"{user.Username}'s Server"
            };

            // add the initial player.
            server.Players.Add(host);

            ReservedUsers.Add(user.Id, server.Id);
            ReservedChannels.Add(channel.Id, server.Id); // server.Config.Title, server.Id, server.Config.GameId, server.Players.Count\

            DisplayChannel display = server.GetDisplayChannel(GameState.Waiting);
            DisplayChannel editing = server.GetDisplayChannel(GameState.Editing);
            string playerLimitCounter = server.Config.ValidateGame() ?
                $"{server.Config.GetGame().Details.PlayerLimit} {OriFormat.TryPluralize("player", server.Config.GetGame().Details.PlayerLimit)}"
                : "infinite players";

            /*
            if (server.Config.ValidateGame())
            {
                if (server.Config?.GetGame()?.Details?.PlayerLimit != null)
                {
                    playerLimitCounter = $"{server.Config.GetGame().Details.PlayerLimit} {OriFormat.GetNounForm("player", server.Config.GetGame().Details.PlayerLimit)}";
                }
            }*/

            display
                .Content
                .GetComponent("header")
                .Draw(server.Config.Title, server.Id, server.Config.GameId, server.Players.Count, playerLimitCounter);

            (display.Content.GetComponent("message_box") as ComponentGroup)
                .Append($"[Console] {user.Username} has joined.");

            (editing.Content.GetComponent("message_box") as ComponentGroup)
                .Append($"[Console] {user.Username} has joined.");

            display.Content.GetComponent("message_box").Draw();
            editing.Content.GetComponent("message_box").Draw(server.Config.Title);

            var internalMessage = await channel.SendMessageAsync(display.ToString());

            ServerConnection connection = new ServerConnection
            {
                InternalChannel = channel,
                ChannelId = channel.Id,
                Frequency = 0,
                State = GameState.Waiting,
                InternalMessage = internalMessage,
                MessageId = internalMessage.Id,
                LastRefreshed = DateTime.UtcNow
            };

            server.Connections.Add(connection);

            // refreshes all displays, just in case
            await server.UpdateAsync();

            Servers.Add(server.Id, server);
        }

        // destroys the game server accordingly
        internal async Task DestroyServerAsync(GameServer server)
        {
            server.GetDisplayChannel(GameState.Waiting).Content.ValueOverride = "This server has been destroyed. Sorry about the inconvenience.";

            // update the display to notify that the server was destroyed
            foreach (ServerConnection connection in server.Connections)
            {
                connection.State = GameState.Waiting;
            }

            await server.UpdateAsync();

            foreach (ulong channelId in server.Connections.Select(x => x.ChannelId))
                if (ReservedChannels.ContainsKey(channelId))
                    ReservedChannels.Remove(channelId);

            foreach (ulong userId in server.Players.Select(x => x.User.Id))
                if (ReservedUsers.ContainsKey(userId))
                    ReservedUsers.Remove(userId);

            Servers.Remove(server.Id);
            // release all channels that were reserved to this server
        }

        // forcibly destroy the current session a server might have


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
            => await OnReaction(message, channel, reaction, ReactionFlag.Add);

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
            => await OnReaction(message, channel, reaction, ReactionFlag.Remove);

        private async Task OnReaction(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction,
            ReactionFlag flag)
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

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == channel.Id);

                // check if the reaction was appended to the right message
                if (connection.MessageId != reaction.MessageId)
                    return;

                DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                foreach (IInput input in display.Inputs)
                {
                    InputResult result = input.TryParse(new Input { Reaction = reaction.Emote, Flag = flag });

                    if (result.IsSuccess)
                    {
                        IUser user = reaction.User.GetValueOrDefault();

                        if (user == null)
                            return;

                        if (result.Input.Criterion?.Invoke(user, connection, server) ?? true)
                        {
                            result.Input.OnExecute?.Invoke(user, connection, server);

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

                            if (result.Input.UpdateOnExecute)
                                await server.UpdateAsync();
                        }

                        // end the cycling of inputs when once is successful
                        return;
                    }
                }
            }
        }

        public async Task OnMessageReceived(SocketMessage message)
        {
            IUser user = message.Author;

            // ignore all bots
            if (user.IsBot)
                return;

            // if this message was sent in a channel that contains a server and the user is in a server
            if (ReservedChannels.ContainsKey(message.Channel.Id))
            {
                // if the user is currently in a server, check to see if they are in the same server
                // otherwise, ignore them because they are meant for another game
                if (ReservedUsers.ContainsKey(user.Id))
                    if (ReservedChannels[message.Channel.Id] != ReservedUsers[user.Id])
                        return;

                GameServer server = Servers[ReservedChannels[message.Channel.Id]];

                Player player = server.GetPlayer(user.Id);

                ServerConnection connection = server.Connections
                    .First(x => x.ChannelId == message.Channel.Id);

                // This is where input handling starts to diverge:
                string ctx = message.Content;

                /*
                // if this connection is able to delete messages, automatically remove it
                // only if the message sent was a valid command
                if (connection.CanDeleteMessages)
                {
                    await message.DeleteAsync();
                }*/

                // get the correlated channel for the specific game state.
                DisplayContent waiting = server.GetDisplayChannel(GameState.Waiting).Content;
                DisplayContent editing = server.GetDisplayChannel(GameState.Editing).Content;

                // if true, update all editing components as well
                bool isEditing = true;

                // check the session state
                // if SessionState.Finish OR SessionState.Destroy
                // get rid of the session.
                if (server.Session != null)
                {
                    if (server.Session.State == SessionState.Destroy)
                    {
                        server.DestroyCurrentSession();
                    }
                    else if (server.Session.State == SessionState.Finish)
                    {
                        // for now, we don't worry about this.
                        SessionResult result = server.Session._game.OnSessionFinish(server.Session);
                        server.DestroyCurrentSession();
                    }
                }

                // to make things easier
                // i need an internal formatting handler that makes appending to multiple components easier
                // likewise, i should extract the more recent values from the lobby, and set them to be new values for the sub info
                switch (connection.State)
                {
                    // IF GAMESTATE IS WAITING
                    // They are in the lobby.
                    case GameState.Waiting:

                        // [HOST, PLAYER] start
                        if (ctx == "start")
                        {
                            string notice = "";

                            // if they are not in the server
                            if (player == null)
                                return;

                            // if they are not the host
                            else if (!player.Host)
                                notice = $"[To {user.Username}] Only the host may start the game.";

                            // if the game already started
                            else if (server.Session != null)
                                notice = $"[Console] A game is already in progress.";

                            // if the game specified is invalid
                            else if (!server.Config.ValidateGame())
                            {
                                notice = $"[Console] Unable to validate the specified game.";
                            }
                            else
                            {
                                GameBuilder game = server.Config.GetGame();

                                if (game != null)
                                {
                                    if (game.Details != null)
                                    {
                                        if (!game.Details.CanStart(server.Players.Count))
                                        {
                                            notice = $"[Console] Unable to start {game.Details.Name ?? "UNKNOWN_NAME"}.";
                                        }
                                        else
                                        {
                                            game.Build(server);
                                            return;
                                            //notice = $"[Console] The session builder is currently in development. Unable to start.";
                                        }
                                    }
                                }
                                else
                                {
                                    notice = $"[Console] An error has occurred attempting to start '{server.Config.GameId}'.";
                                }
                            }

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                             waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            break;
                        }

                        // join
                        if (ctx == "join")
                        {
                            // if they are already in the server
                            if (player != null)
                                return;

                            // otherwise, add them to the players and update accordingly
                            var newPlayer = new Player
                            {
                                User = user,
                                JoinedAt = DateTime.UtcNow,
                                Host = false,
                                Playing = false
                            };

                            server.Players.Add(newPlayer);
                            ReservedUsers.Add(user.Id, server.Id);

                            string notice = $"[Console] {user.Username} has joined.";

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            string playerLimitCounter = server.Config.ValidateGame() ?
                                $"{server.Config.GetGame().Details.PlayerLimit} {OriFormat.TryPluralize("player", server.Config.GetGame().Details.PlayerLimit)}"
                                : "infinite players";

                            waiting.GetComponent("header")
                                .Draw(server.Config.Title,
                                    server.Id,
                                    server.Config.GameId,
                                    server.Players.Count,
                                    playerLimitCounter);

                            break;
                        }

                        // [PLAYER] leave
                        if (ctx == "leave")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            // otherwise, remove them from the players and update accordingly
                            server.Players.Remove(player);
                            ReservedUsers.Remove(user.Id);

                            // if the player that left was the final player in the server
                            if (server.Players.Count == 0)
                            {
                                await DestroyServerAsync(server);
                                return;
                            }

                            string playerLimitCounter = server.Config.ValidateGame() ?
                                $"{server.Config.GetGame().Details.PlayerLimit} {OriFormat.TryPluralize("player", server.Config.GetGame().Details.PlayerLimit)}"
                                : "infinite players";

                            waiting.GetComponent("header")
                                .Draw(server.Config.Title,
                                    server.Id,
                                    server.Config.GameId,
                                    server.Players.Count,
                                    playerLimitCounter);

                            string leaveNotice = $"[Console] {user.Username} has left.";

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(leaveNotice);

                            if (isEditing)
                                (editing.GetComponent("message_box") as ComponentGroup).Append(leaveNotice);

                            // if the player that left was the host
                            if (player.Host)
                            {
                                var newHost = server.Players.OrderBy(x => x.JoinedAt).First();
                                newHost.Host = true;

                                string hostNotice = $"[Console] {newHost.User.Username} is now the host.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(hostNotice);

                                if (isEditing)
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(hostNotice);

                                if (server.Connections.Any(x => x.State == GameState.Editing))
                                {
                                    // if the host was re-assigned, return the channel in editing state to it's default
                                    server.Connections.First(x => x.State == GameState.Editing).State = GameState.Waiting;
                                }
                            }

                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                                editing.GetComponent("message_box").Draw(server.Config.Title);

                            break;
                        }


                        // [PLAYER] refresh
                        // brings the lobby up to the most recent message

                        if (ctx == "refresh")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            /*
                            // if they are not the host
                            if (!player.Host)
                            {
                                (content.GetComponent("message_box") as ComponentGroup)
                                    .Append($"[To {user.Username}] Only the host may refresh the display window.");

                                content.GetComponent("message_box").Draw();

                                break;
                            }*/

                            await connection.InternalMessage.DeleteAsync();

                            connection.InternalMessage = await connection.InternalChannel.SendMessageAsync(waiting.ToString());
                            connection.MessageId = connection.InternalMessage.Id;

                            return;
                            // otherwise, bring the display window up to the front again

                        }

                        // [PLAYER] players
                        if (ctx == "players")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            // otherwise, send a message to the console that lists all players.
                            string buffer = $"Players ({server.Players.Count}): {string.Join(", ", server.Players.Select(x => $"{x.User.Username} {(x.Host ? "[Host]" : "")}{(x.Playing ? " - Playing" : "")}"))}";

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(buffer);
                             waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                 editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            break;
                        }

                        if (ctx == "connections")
                        {
                            if (player == null)
                                return;

                            string buffer = "";

                            // if they are not the host
                            if (!player.Host)
                            {
                                buffer = $"[To {user.Username}] Only the host may list server connections.";
                            }
                            else
                            {
                                buffer = $"Connections ({server.Connections.Count}): {string.Join(", ", server.Connections.Select(x => $"{x.ChannelId} - {x.State.ToString()}"))}";
                            }

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(buffer);
                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            break;
                        }

                        // [HOST, PLAYER] config
                        if (ctx == "config")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            string notice = "";

                            // if they are not the host
                            if (!player.Host)
                            {
                                notice = $"[To {user.Username}] Only the host can edit the server config.";
                            }

                            // if there is an existing connection already in the editing state
                            else if (server.Connections.Any(x => x.State == GameState.Editing))
                            {
                                notice = $"[Console] There is already a connection that is editing the configuration.";
                            }
                            else
                            {
                                // otherwise, set this specific server connection to editing mode.
                                connection.State = GameState.Editing;
                                connection.Frequency = 1;

                                if (server.Config.ValidateGame())
                                {
                                    editing.GetComponent("game_config").Active = true;
                                    editing.GetComponent("game_config").Draw(server.Config.GetGame().Config.Select(x => $"**{x.Name}**: `{x.Value.ToString()}`"), server.Config.GetGame().Details.Name);
                                }

                                editing.GetComponent("message_box").Draw(server.Config.Title);
                                editing.GetComponent("config").Draw(server.Config.Title, server.Config.Privacy, server.Config.GameId);

                                break;
                            }

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                             waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                 editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            break;
                        }

                        // [PLAYER] watch
                        if (ctx == "watch")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            string buffer = "";

                            // if the game hasn't started yet
                            if (server.Session == null)
                            {

                                buffer = $"[Console] Unable to call vote for spectating as there is no current session.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                 waiting.GetComponent("message_box").Draw();

                                if (isEditing)
                                {
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                     editing.GetComponent("message_box").Draw(server.Config.Title);
                                }

                                break;
                            }

                            // if they are the host, automatically start it.
                            if (player.Host)
                            {
                                buffer = $"[Console] The spectator controls are currently in development. Unable to initialize.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                waiting.GetComponent("message_box").Draw();

                                if (isEditing)
                                {
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                    editing.GetComponent("message_box").Draw(server.Config.Title);
                                }

                                break;
                            }

                            buffer = $"[Console] The spectator controls are currently in development. Unable to initialize.";

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(buffer);
                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            break;
                        }

                        return;

                    // IF GAMESTATE IS EDITING
                    case GameState.Editing:
                        // base commands

                        // join
                        if (ctx == "join")
                        {
                            // if they are already in the server
                            if (player != null)
                                return;

                            // otherwise, add them to the players and update accordingly
                            var newPlayer = new Player
                            {
                                User = user,
                                JoinedAt = DateTime.UtcNow,
                                Host = false,
                                Playing = false
                            };

                            server.Players.Add(newPlayer);
                            ReservedUsers.Add(user.Id, server.Id);

                            string notice = $"[Console] {user.Username} has joined.";

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            string playerLimitCounter = server.Config.ValidateGame() ?
                                $"{server.Config.GetGame().Details.PlayerLimit} {OriFormat.TryPluralize("player", server.Config.GetGame().Details.PlayerLimit)}"
                                : "infinite players";

                            waiting.GetComponent("header")
                                .Draw(server.Config.Title,
                                    server.Id,
                                    server.Config.GameId,
                                    server.Players.Count,
                                    playerLimitCounter);

                            break;
                        }

                        // [PLAYER] leave
                        if (ctx == "leave")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            // otherwise, remove them from the players and update accordingly
                            server.Players.Remove(player);
                            ReservedUsers.Remove(user.Id);

                            // if the player that left was the final player in the server
                            if (server.Players.Count == 0)
                            {
                                await DestroyServerAsync(server);
                                return;
                            }

                            string playerLimitCounter = server.Config.ValidateGame() ?
                                $"{server.Config.GetGame().Details.PlayerLimit} {OriFormat.TryPluralize("player", server.Config.GetGame().Details.PlayerLimit)}"
                                : "infinite players";

                            waiting.GetComponent("header")
                                .Draw(server.Config.Title,
                                    server.Id,
                                    server.Config.GameId,
                                    server.Players.Count,
                                    playerLimitCounter);

                            string leaveNotice = $"[Console] {user.Username} has left.";

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(leaveNotice);

                            if (isEditing)
                                (editing.GetComponent("message_box") as ComponentGroup).Append(leaveNotice);

                            // if the player that left was the host
                            if (player.Host)
                            {
                                var newHost = server.Players.OrderBy(x => x.JoinedAt).First();
                                newHost.Host = true;

                                string hostNotice = $"[Console] {newHost.User.Username} is now the host.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(hostNotice);

                                if (isEditing)
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(hostNotice);

                                if (server.Connections.Any(x => x.State == GameState.Editing))
                                {
                                    // if the host was re-assigned, return the channel in editing state to it's default
                                    server.Connections.First(x => x.State == GameState.Editing).State = GameState.Waiting;
                                }
                                
                            }

                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                                editing.GetComponent("message_box").Draw(server.Config.Title);

                            break;
                        }

                        // [HOST, PLAYER] connections
                        if (ctx == "connections")
                        {
                            if (player == null)
                                return;

                            string buffer = "";

                            // if they are not the host
                            if (!player.Host)
                            {
                                buffer = $"[To {user.Username}] Only the host may list server connections.";
                            }
                            else
                            {
                                buffer = $"Connections ({server.Connections.Count}): {string.Join(", ", server.Connections.Select(x => $"{x.ChannelId} - {x.State.ToString()}"))}";
                            }

                            (waiting.GetComponent("message_box") as ComponentGroup).Append(buffer);
                            waiting.GetComponent("message_box").Draw();

                            if (isEditing)
                            {
                                (editing.GetComponent("message_box") as ComponentGroup).Append(buffer);
                                editing.GetComponent("message_box").Draw(server.Config.Title);
                            }

                            break;
                        }
                        // [HOST, PLAYER] title <value>
                        // [HOST, PLAYER] game <value>
                        // [HOST, PLAYER] privacy <value>
                        // [HOST, PLAYER] kick <value>

                        // This returns the host back into the normal lobby view
                        // [HOST, PLAYER] back
                        if (ctx == "back")
                        {
                            // if they are already in the server
                            if (player == null)
                                return;

                            // if they are not the host
                            if (!player.Host)
                            {
                                string notice = $"[To {user.Username}] Only the host may exit server configuration.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                                 waiting.GetComponent("message_box").Draw();

                                if (isEditing)
                                {
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                     editing.GetComponent("message_box").Draw(server.Config.Title);
                                }

                                break;
                            }

                            connection.State = GameState.Waiting;
                            connection.Frequency = 0;

                            //waiting.GetComponent("message_box").Draw();
                            //editing.GetComponent("message_box").Draw(server.Config.Title);

                            break;
                        }

                        
                        if (ctx.StartsWith("title"))
                        {
                            // if they are already in the server
                            if (player != null)
                                return;

                            // if they are not the host
                            if (!player.Host)
                            {
                                string notice = $"[To {user.Username}] Only the host may edit the server's title.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                                 waiting.GetComponent("message_box").Draw();

                                if (isEditing)
                                {
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                     editing.GetComponent("message_box").Draw(server.Config.Title);
                                }

                                break;
                            }

                            break;
                        }

                        if (ctx.StartsWith("game"))
                        {
                            // if they are already in the server
                            if (player != null)
                                return;

                            // if they are not the host
                            if (!player.Host)
                            {
                                string notice = $"[To {user.Username}] Only the host may edit the server's game mode.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                                 waiting.GetComponent("message_box").Draw();

                                if (isEditing)
                                {
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                     editing.GetComponent("message_box").Draw(server.Config.Title);
                                }

                                break;
                            }

                            break;
                        }

                        if (ctx.StartsWith("privacy"))
                        {
                            // if they are already in the server
                            if (player != null)
                                return;

                            // if they are not the host
                            if (!player.Host)
                            {
                                string notice = $"[To {user.Username}] Only the host may edit the server's privacy.";

                                (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                                 waiting.GetComponent("message_box").Draw();

                                if (isEditing)
                                {
                                    (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                     editing.GetComponent("message_box").Draw(server.Config.Title);
                                }

                                break;
                            }

                            break;
                        }

                        // if no other commands are valid, attempt to parse a custom configuration
                        // otherwise, just return.
                        // [HOST, PLAYER] CUSTOM <value>
                        if (server.Config.ValidateGame())
                        {
                            var game = server.Config.GetGame();

                            foreach(ConfigProperty option in game.Config)
                            {
                                if (ctx.StartsWith(option.Id))
                                {
                                    if (player == null)
                                        return;

                                    if (!player.Host)
                                    {
                                        string notice = $"[To {user.Username}] Only the host may edit this value.";

                                        (waiting.GetComponent("message_box") as ComponentGroup).Append(notice);
                                        waiting.GetComponent("message_box").Draw();

                                        if (isEditing)
                                        {
                                            (editing.GetComponent("message_box") as ComponentGroup).Append(notice);
                                            editing.GetComponent("message_box").Draw(server.Config.Title);
                                        }

                                        break;
                                    }

                                    break;
                                }
                            }

                            return;
                        }

                        return;

                    // IF GAMESTATE IS WATCHING
                    case GameState.Watching:
                        // [HOST, PLAYER] end
                        if (ctx == "end")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            // if they are the host, force end the current session

                            // otherwise, send a note stating that they cannot force end a session unless
                            // they are a host

                            return;
                            //break;
                        }

                        // [PLAYER] back
                        if (ctx == "back")
                        {
                            // if they are not in the server
                            if (player == null)
                                return;

                            // if they are the host, automatically call

                            // otherwise, begin calling the vote to switch over to spectator mode.

                            return;
                            //break;
                        }

                        return;

                    // IF GAMESTATE IS PLAYING
                    case GameState.Playing:
                        

                        // otherwise, if they are currently in an active game, simply attempt to parse the inputs
                        // specified by the game session themselves
                        // and refer to frequency instead
                        DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                        foreach (IInput input in display.Inputs)
                        {
                            var rawCtx = ctx;
                            if (input.Type == KeyType.Text)
                            {
                                if (!(input as TextInput).CaseSensitive)
                                {
                                    rawCtx = rawCtx.ToLower();
                                }
                            }

                            InputResult result = input.TryParse(rawCtx);
                            if (result.IsSuccess)
                            {
                                

                                if (result.Input.Criterion?.Invoke(user, connection, server) ?? true)
                                {
                                    result.Input.OnExecute?.Invoke(user, connection, server);

                                    // if there wasn't a player before and they were added, add them to the reserves
                                    if (player == null)
                                    {
                                        if (server.GetPlayer(user.Id) != null)
                                        {
                                            ReservedUsers[user.Id] = server.Id;
                                        }
                                    }
                                    else // likewise, if there was a player and they were removed, remove them from the reserves
                                    {
                                        if (server.GetPlayer(user.Id) == null)
                                        {
                                            if (ReservedUsers.ContainsKey(user.Id))
                                                ReservedUsers.Remove(user.Id);

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

                                    if (result.Input.UpdateOnExecute)
                                        await server.UpdateAsync();
                                }

                                // end the cycling of inputs when one is successful
                                return;
                            }
                        }
                        break;
                }

                await server.UpdateAsync();
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

/*
 
    Servers
    JoinServerAsync(IUser user, IMessageChannel channel, GameServer server)
        - Attempts to add a player into a server
    
    LeaveServerAsync(IUser user, IMessageChannel channel)
        - Attempts to remove a player from an existing server
        - If they were the host, set a new host
        - If they were the last person in the server, destroy the server
        - If they were the last person to a connection, remove the connection


    CreateServerAsync(IUser user, IMessageChannel channel)
        - Creates a new server with the user as the host
        - If a user is already in another server, cancel the attempt
        - If the channel is already reserved, cancel the attempt
        - If the server connection was initialized in a guild, attach a GuildId to the connection
        - That way, if another connection to the same server is initialized in the same guild, it can be prevented.


    DestroyServerAsync(GameServer server)
        - Destroys an existing server
        - Forcibly moves everyone to the default state
        - Releases all reserved users and channels
    
    AddPlayerAsync(Player player)
        - Attempts to add a player into an existing server
        - If the server is full, they are unable to join



    RemovePlayerAsync(Player player, string reason = null)
        - If the reason is null, the player will be assumed as willingly left.
        - Otherwise, the player will have been marked as kicked
        - This attempts to remove the player from the existing server they are in
        - If they are not in a server, cancel the attempt
    
    AddConnectionAsync(IMessageChannel channel, GameServer server)
        - This establishes a new connection to a server by a specified channel
        - All new connections must default to GameState.Waiting
        

    RemoveConnectionAsync(ServerConnection connection)
        - This forcibly removes a connection from a server with a specified channel
        - If this was the last connection to the server, destroy the server
        - If the connection removed was the only channel the host had access to, set a new host
        - Cancel a session if this method is executed on a channel with the state GameState.Playing

    Handles
    OnChannelDestroyed
    
    OnReactionAdded
    
    OnReactionRemoved
    
    OnMessageReceived

    OnMessageDeleted
     
     
     
 */