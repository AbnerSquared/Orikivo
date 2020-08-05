using Arcadia.Multiplayer.Games;
using Discord;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Format = Orikivo.Format;

namespace Arcadia.Multiplayer

{
    internal static class LobbyVars
    {
        internal static readonly string Header = "header";
        internal static readonly string Console = "console";
    }

    internal static class ServerCommand
    {
        internal static readonly string Start = "start";
        internal static readonly string Join = "join";
        internal static readonly string Leave = "leave";
        internal static readonly string Config = "config";
        internal static readonly string Watch = "watch";
        internal static readonly string Back = "back";
        internal static readonly string Host = "host";
        internal static readonly string Players = "players";
        internal static readonly string Connections = "connections";
        internal static readonly string Privacy = "privacy";
        internal static readonly string Invite = "invite";
    }

    public class GameManager
    {
        private readonly DiscordSocketClient _client;
        private readonly ArcadeContainer _container;

        public GameManager(DiscordSocketClient client, ArcadeContainer container)
        {
            _client = client;
            _container = container;
            Servers = new Dictionary<string, GameServer>();
            ReservedChannels = new Dictionary<ulong, string>();
            ReservedUsers = new Dictionary<ulong, string>();

            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            _client.MessageReceived += OnMessageReceived;
            _client.MessageDeleted += OnMessageDeleted;
        }

        internal static Dictionary<string, GameBase> Games => new Dictionary<string, GameBase>
        {
            ["Trivia"] = new TriviaGame(),
            ["Werewolf"] = new WerewolfGame()
        };

        internal Dictionary<string, GameServer> Servers { get; set; }

        // this is all channels that are currently bound to a game server
        internal Dictionary<ulong, string> ReservedChannels { get; set; }

        internal Dictionary<ulong, string> ReservedUsers { get; set; }

        public IEnumerable<GameServer> GetPublicServers()
            => Servers.Values.Where(x => x.Config.Privacy == Privacy.Public);

        public static GameBase GetGame(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Games[gameId];
        }

        // This attempts to grab a random server ID
        public string GetRandomServer()
        {
            // Randomizer.Choose(IEnumerable<T> set, Func<T, bool> predicate)
            if (Servers.Values.All(x => x.IsFull))
                return null;

            return Randomizer.Choose(Servers.Values.Where(x => x.Config.Privacy == Privacy.Public && !x.IsFull)).Id;
        }

        // This attempts to grab a random server ID for the specified game mode
        // Quick-join games that have room for players
        public string GetRandomServer(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            if (Servers.Values.All(x => x.IsFull))
                return null;

            return Randomizer.Choose(Servers.Values.Where(x => x.Config.Privacy == Privacy.Public && !x.IsFull && x.Config.GameId == gameId)).Id;
        }

        // This attempts to join an existing server, if one is present
        public async Task JoinServerAsync(IUser user, IMessageChannel channel, string serverId, IGuild guild = null)
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
            
            var player = new Player
            {
                Host = false,
                User = user,
                JoinedAt = DateTime.UtcNow,
                Playing = false
            };

            server.Players.Add(player);
            ReservedUsers.Add(user.Id, server.Id);

            RefreshConsoleHeader(server);
            AppendToConsole(server, $"[Console] {user.Username} has joined.");

            // if there's a connection already in place, simply update this message.
            ServerConnection connection = server.Connections.FirstOrDefault(x => x.ChannelId == channel.Id);

            // otherwise, if there isn't a connection, establish a new one.
            if (connection == null)
            {
                var properties = new ConnectionProperties
                {
                    AutoRefreshCounter = 4,
                    BlockInput = false,
                    CanDeleteMessages = true,
                    Frequency = 0,
                    State = GameState.Waiting
                };

                connection = await ServerConnection.CreateAsync(channel, server.GetDisplayChannel(GameState.Waiting), properties);
                server.Connections.Add(connection);
                ReservedChannels.Add(channel.Id, server.Id);
            }

            await server.UpdateAsync();
        }

        public static GameDetails DetailsOf(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Games[gameId].Details;
        }

        private void RefreshConsoleHeader(GameServer server)
        {
            GameDetails details = DetailsOf(server.Config.GameId);

            string playerCounter = $"{server.Players.Count:##,0} {Format.TryPluralize("player", server.Players.Count)}";

            if (details != null)
            {
                playerCounter = $"{server.Players.Count:##,0}/{details.PlayerLimit:##,0} {Format.TryPluralize("player", details.PlayerLimit)}";
            }

            server
                .GetDisplayChannel(GameState.Waiting)
                .GetComponent(LobbyVars.Header)
                .Draw(server.Config.Name, server.Id, server.Config.GameId, playerCounter);
        }

        // this starts a new base game server
        public async Task CreateServerAsync(IUser user, IMessageChannel channel, IGuild guild = null, string gameId = null)
        {
            // If unspecified get the default game mode.
            gameId ??= "Trivia";

            // Ignore all bots
            if (user.IsBot)
                return;

            // If the user or channel is reserved, throw an error
            if (ReservedUsers.ContainsKey(user.Id) || ReservedChannels.ContainsKey(channel.Id))
                throw new Exception("Cannot initialize a new server as either the user or a channel is reserved to an existing server");

            // Create the player and host of the server
            var host = new Player
            {
                Host = true,
                User = user,
                JoinedAt = DateTime.UtcNow,
                Playing = false
            };

            // Initialize the new server
            var server = new GameServer(this)
            {
                Config = new ServerProperties
                {
                    Privacy = Privacy.Public,
                    GameId = gameId,
                    Name = $"{user.Username}'s Server"
                }
            };

            // Add the creator of the server
            server.Players.Add(host);

            ReservedUsers.Add(user.Id, server.Id);
            ReservedChannels.Add(channel.Id, server.Id); // server.Config.Title, server.Id, server.Config.GameId, server.Players.Count\

            RefreshConsoleHeader(server);
            AppendToConsole(server, $"[Console] {user.Username} has joined.");

            var properties = new ConnectionProperties
            {
                AutoRefreshCounter = 4,
                BlockInput = false,
                CanDeleteMessages = true,
                Frequency = 0,
                State = GameState.Waiting
            };

            var connection = await ServerConnection.CreateAsync(channel, server.GetDisplayChannel(GameState.Waiting), properties);
            connection.Origin = OriginType.Server;

            if (guild != null)
            {
                connection.Type = ConnectionType.Guild;
                connection.GuildId = guild.Id;
            }

            await EnsureDeleteAsync(connection);

            if (guild != null)
                connection.GuildId = guild.Id;

            server.Connections.Add(connection);
            Servers.Add(server.Id, server);
        }

        internal void EndSession(GameSession session)
        {
            SessionResult result = session.Game.OnSessionFinish(session);
            result.Apply(_container);
        }

        // Destroys the session of a game server
        internal async Task DestroySessionAsync(GameServer server)
        {
            if (server.Session != null)
            {
                server.DestroyCurrentSession();
                AppendToConsole(server, "[Console] The current session has been destroyed.");
            }
        }

        // destroys the game server accordingly
        internal async Task DestroyServerAsync(GameServer server)
        {
            // Override the display to specify that the server was destroyed
            server.GetDisplayChannel(GameState.Waiting).Content.ValueOverride = $"> ⚠️ **{server.Config.Name}** has been shut down.\n> Sorry about the inconvenience.";

            // Set all connections to the default state
            foreach (ServerConnection connection in server.Connections)
            {
                // Handle the deletion of all connections created during a session, if any
                if (server.Session != null)
                {
                    if (connection.Origin == OriginType.Session)
                        await connection.DestroyAsync();
                    else if (connection.CreatedAt > server.Session.StartedAt && connection.Origin == OriginType.Unknown)
                        await connection.DestroyAsync();
                }

                connection.State = GameState.Waiting;
            }

            // Update all connection to reflect the changes made
            await server.UpdateAsync();

            // Release all channels reserved to this server
            foreach (ulong channelId in server.Connections.Select(x => x.ChannelId))
                if (ReservedChannels.ContainsKey(channelId))
                    ReservedChannels.Remove(channelId);

            // Release all users reserved to this server
            foreach (ulong userId in server.Players.Select(x => x.User.Id))
                if (ReservedUsers.ContainsKey(userId))
                    ReservedUsers.Remove(userId);

            // Remove and delete the server
            Servers.Remove(server.Id);
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
            => await OnReaction(message, channel, reaction, ReactionHandling.Add);

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
            => await OnReaction(message, channel, reaction, ReactionHandling.Remove);

        private async Task OnReaction(
            Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction,
            ReactionHandling flag)
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

                // If the current connection doesn't allow input, ignore it
                if (connection.BlockInput)
                    return;

                // If the entire session doesn't allow input, ignore it
                if (server.Session != null)
                    if (server.Session.BlockInput)
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
                            result.Input.OnExecute?.Invoke(new InputContext(user, connection, server, result));

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

        // Refreshes the server's configuration details
        private static void RefreshServerConfig(GameServer server)
        {
            server.GetDisplayChannel(GameState.Editing)
                .Content.GetComponent("config")
                .Draw(server.Config.Name, server.Config.Privacy, server.Config.GameId);
        }

        private static void RefreshGameConfig(GameServer server)
        {
            DisplayContent editing = server.GetDisplayChannel(GameState.Editing).Content;

            if (!server.Config.IsValidGame())
                return;

            server.Config.LoadGame();

            if (!Check.NotNullOrEmpty(server.Config.GameOptions))
                return;

            editing.GetComponent("game_config").Active = true;
            editing.GetComponent("game_config").Draw(
                server.Config.GameOptions.Select(x => $"**{x.Name}**: `{x.Value}`"),
                DetailsOf(server.Config.GameId).Name);
        }

        private static void RefreshConsole(GameServer server)
        {
            server.GetDisplayChannel(GameState.Editing).Content.GetComponent(LobbyVars.Console).Draw(server.Config.Name);
            server.GetDisplayChannel(GameState.Waiting).Content.GetComponent(LobbyVars.Console).Draw();
        }

        // Appends the specified message to all of the reserved console components and updates accordingly
        private static void AppendToConsole(GameServer server, string message, bool draw = true)
        {
            DisplayContent waiting = server.GetDisplayChannel(GameState.Waiting).Content;
            DisplayContent editing = server.GetDisplayChannel(GameState.Editing).Content;

            waiting.GetGroup(LobbyVars.Console).Append(message);
            editing.GetGroup(LobbyVars.Console).Append(message);

            if (!draw)
                return;

            editing.GetComponent(LobbyVars.Console).Draw(server.Config.Name);
            waiting.GetComponent(LobbyVars.Console).Draw();
        }

        private static string WritePlayerInfo(Player player)
        {
            var info = new StringBuilder();

            info.Append(player.User.Username);

            if (player.Host)
                info.Append(" [Host]");

            if (player.Playing)
                info.Append(" - Playing");

            return info.ToString();
        }
        public async Task OnMessageReceived(SocketMessage message)
        {
            var allowUpdate = true;
            IUser user = message.Author;

            // Ignore all bots
            if (user.IsBot)
                return;

            // Ignore if the origin of this message is not in the reserves
            if (!ReservedChannels.ContainsKey(message.Channel.Id))
                return;

            // Ignore if the user is already reserved but the servers references don't match
            if (ReservedUsers.ContainsKey(user.Id))
                if (ReservedChannels[message.Channel.Id] != ReservedUsers[user.Id])
                    return;

            // Throw an error if the reserved reference points to an empty server
            if (!Servers.ContainsKey(ReservedChannels[message.Channel.Id]))
                throw new Exception("Unable to find the requested server for the specified channel");

            GameServer server = Servers[ReservedChannels[message.Channel.Id]];

            // Throw an error if the specified channel saved in the reserves doesn't exist on this server
            if (!server.HasConnection(message.Channel.Id))
                throw new Exception("Unable to retrieve the server connection for the specified channel");

            Player player = server.GetPlayer(user.Id);
            ServerConnection connection = server.Connections.First(x => x.ChannelId == message.Channel.Id);

            // If the current connection doesn't allow input, ignore it
            if (connection.BlockInput)
                return;

            // Extract only the content of the message
            string ctx = message.Content;

            // Load all of the display channels for the specified game states
            DisplayContent waiting = server.GetDisplayChannel(GameState.Waiting).Content;
            DisplayContent editing = server.GetDisplayChannel(GameState.Editing).Content;

            // Check if the session exists
            if (server.Session != null)
            {
                // Handle any closing states for a session
                switch (server.Session.State)
                {
                    case SessionState.Destroy:
                        server.DestroyCurrentSession();
                        break;

                    case SessionState.Finish:
                        EndSession(server.Session);
                        server.DestroyCurrentSession();
                        break;
                }
                
                // Otherwise, ignore if the session doesn't allow input
                if (server.Session.BlockInput)
                    return;
            }

            allowUpdate = player != null;

            // Handle the input given based on the server connection state
            switch (connection.State)
            {
                // This represents the primary lobby
                case GameState.Waiting:

                    // [HOST, PLAYER] start
                    if (ctx == "start")
                    {
                        var notice = "";

                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        // Ignore if they are not the host
                        if (!player.Host)
                        {
                            notice = $"[To {user.Username}] Only the host may start the game.";
                            AppendToConsole(server, notice);
                            break;
                        }

                        GameDetails details = DetailsOf(server.Config.GameId);

                        // Ignore if the game already started
                        if (server.Session != null)
                            notice = $"[Console] A game is already in progress.";

                        // Ignore if the game is invalid
                        else if (!server.Config.IsValidGame())
                            notice = $"[Console] Unable to validate the specified game.";

                        // Ignore if the details of the game don't exist
                        else if (details == null)
                            notice = $"[Console] Unable to start a session '{server.Config.GameId}'.";

                        // Otherwise, attempt to start the game
                        else
                        {
                            string gameName = details.Name ?? "UNKNOWN_GAME";

                            // Try to start the game
                            if (server.Players.Count >= details.RequiredPlayers && server.Players.Count <= details.PlayerLimit)
                            {
                                try
                                {
                                    await server.Config.LoadGame().BuildAsync(server);
                                    return;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    notice = $"[Console] An exception has been thrown while initializing {gameName}.";
                                    server.DestroyCurrentSession();
                                    return;
                                }
                            }

                            if (server.Players.Count >= details.PlayerLimit)
                                notice = $"[Console] There are too many players in this server to start {gameName}.";
                            else if (server.Players.Count < details.RequiredPlayers)
                            {
                                int requiredPlayers = details.RequiredPlayers - server.Players.Count;
                                string conjoin = requiredPlayers > 1 ? "are" : "is";
                                notice = $"[Console] {requiredPlayers} more {Format.TryPluralize("player", requiredPlayers)} {conjoin} required to start {gameName}.";
                            }
                        }

                        AppendToConsole(server, notice);
                        break;
                    }

                    // join
                    if (ctx == "join")
                    {
                        // Ignore if they already exist
                        if (player != null)
                            break;

                        allowUpdate = true;
                        // Create their information
                        var newPlayer = new Player
                        {
                            User = user,
                            JoinedAt = DateTime.UtcNow,
                            Host = false,
                            Playing = false
                        };
                        
                        // Add them to the server and reserves
                        server.Players.Add(newPlayer);
                        ReservedUsers.Add(user.Id, server.Id);

                        string notice = $"[Console] {user.Username} has joined.";

                        AppendToConsole(server, notice);
                        RefreshConsoleHeader(server);
                        break;
                    }

                    // [PLAYER] leave
                    if (ctx == "leave")
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        // Remove them from the server and reserves
                        server.Players.Remove(player);
                        ReservedUsers.Remove(user.Id);

                        // Destroy the server if the removed player was the only player left
                        if (server.Players.Count == 0)
                        {
                            await DestroyServerAsync(server);
                            return;
                        }

                        RefreshConsoleHeader(server);
                        
                        string leaveNotice = $"[Console] {user.Username} has left.";
                        AppendToConsole(server, leaveNotice, false);

                        // If the player that left was the host
                        if (player.Host)
                        {
                            Player oldest = server.Players.OrderBy(x => x.JoinedAt).FirstOrDefault();
                            
                            if (oldest == null)
                                throw new Exception("Expected player but returned null");

                            oldest.Host = true;
                            
                            string hostNotice = $"[Console] {oldest.User.Username} is now the host.";
                            AppendToConsole(server, hostNotice, false);
                            
                            // If the host was changed, return the channel in editing state to it's default
                            server.ChangeState(GameState.Editing, GameState.Waiting);
                        }

                        RefreshConsole(server);
                        break;
                    }

                    // [HOST, PLAYER] invite <user>

                    // [PLAYER] refresh
                    if (ctx == "refresh")
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        // Refresh the display, if possible
                        await connection.RefreshAsync(server);
                        return;
                    }

                    // [PLAYER] players
                    if (ctx == "players") // TODO: Implement cooldown
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        // This has to handle in the case of too many players being in a server at once
                        string buffer = $"Players ({server.Players.Count}): {string.Join(", ", server.Players.Select(WritePlayerInfo))}";
                        AppendToConsole(server, buffer);
                        break;
                    }

                    // [HOST, PLAYER] connections
                    if (ctx == "connections") // TODO: Implement cooldown
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        string buffer = $"[To {user.Username}] Only the host may view server connections.";

                        // List connections if the player is the host
                        if (player.Host)
                            buffer = $"Connections ({server.Connections.Count}): {string.Join(", ", server.Connections.Select(x => $"{x.ChannelId} - {x.State.ToString()}"))}";

                        AppendToConsole(server, buffer);
                        break;
                    }

                    // [HOST, PLAYER] config
                    if (ctx == "config")
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        var notice = "";

                        // if they are not the host
                        if (!player.Host)
                            notice = $"[To {user.Username}] Only the host can edit the server config.";
                        

                        // if there is an existing connection already in the editing state
                        else if (server.Connections.Any(x => x.State == GameState.Editing))
                            notice = $"[Console] There is already a connection that is editing the configuration.";

                        // Otherwise, set this specific server connection to editing mode.
                        else
                        {
                            connection.State = GameState.Editing;
                            connection.Frequency = 1;

                            if (server.Config.IsValidGame())
                            {
                                if (Check.NotNullOrEmpty(server.Config.GameOptions))
                                {
                                    editing.GetComponent("game_config").Active = true;
                                    editing.GetComponent("game_config").Draw(
                                        server.Config.LoadGame().Options.Select(x =>
                                            $"**{x.Name}**: `{x.Value}`"),
                                        server.Config.LoadGame().Details.Name);
                                }
                            }

                            RefreshGameConfig(server);

                            editing.GetComponent(LobbyVars.Console).Draw(server.Config.Name);
                            editing.GetComponent("config").Draw(server.Config.Name, server.Config.Privacy, server.Config.GameId);
                            break;
                        }

                        AppendToConsole(server, notice);
                        break;
                    }

                    // [PLAYER] watch
                    if (ctx == "watch")
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        var buffer = "";

                        // if the game hasn't started yet
                        if (server.Session == null)
                        {
                            buffer = $"[Console] Unable to call vote for spectating as there is no current session.";
                            AppendToConsole(server, buffer);
                            break;
                        }

                        // if they are the host, automatically start it.
                        if (player.Host)
                        {
                            buffer = $"[Console] The spectator control panel is currently in development. Unable to initialize.";
                            AppendToConsole(server, buffer);
                            break;
                        }

                        // TODO: start up a spectator vote session, and if > 67% of players agree, start spectating
                        buffer = $"[Console] The spectator control panel is currently in development. Unable to initialize.";
                        AppendToConsole(server, buffer);
                        break;
                    }

                    return;

                // This represents the configuration panel
                case GameState.Editing:

                    // join
                    if (ctx == "join")
                    {
                        // Ignore if they already exist
                        if (player == null)
                            break;

                        allowUpdate = true;
                        // Create their information
                        var newPlayer = new Player
                        {
                            User = user,
                            JoinedAt = DateTime.UtcNow,
                            Host = false,
                            Playing = false
                        };

                        // Add them to the server and reserves
                        server.Players.Add(newPlayer);
                        ReservedUsers.Add(user.Id, server.Id);

                        string notice = $"[Console] {user.Username} has joined.";

                        AppendToConsole(server, notice);
                        RefreshConsoleHeader(server);
                        break;
                    }

                    // [PLAYER] leave
                    if (ctx == "leave")
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        // Remove them from the server and reserves
                        server.Players.Remove(player);
                        ReservedUsers.Remove(user.Id);

                        // Destroy the server if the removed player was the only player left
                        if (server.Players.Count == 0)
                        {
                            await DestroyServerAsync(server);
                            return;
                        }

                        RefreshConsoleHeader(server);

                        string leaveNotice = $"[Console] {user.Username} has left.";
                        AppendToConsole(server, leaveNotice, false);

                        // If the player that left was the host
                        if (player.Host)
                        {
                            Player oldest = server.Players.OrderBy(x => x.JoinedAt).FirstOrDefault();

                            if (oldest == null)
                                throw new Exception("Expected player but returned null");

                            oldest.Host = true;

                            string hostNotice = $"[Console] {oldest.User.Username} is now the host.";
                            AppendToConsole(server, hostNotice, false);

                            // If the host was changed, return the channel in editing state to it's default
                            server.ChangeState(GameState.Editing, GameState.Waiting);
                        }

                        RefreshConsole(server);
                        break;
                    }

                    // [HOST, PLAYER] connections
                    if (ctx == "connections") // TODO: Implement cooldown
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        string buffer = $"[To {user.Username}] Only the host may view server connections.";

                        // List connections if the player is the host
                        if (player.Host)
                            buffer = $"Connections ({server.Connections.Count}): {string.Join(", ", server.Connections.Select(x => $"{x.ChannelId} - {x.State.ToString()}"))}";

                        AppendToConsole(server, buffer);
                        break;
                    }

                    // [HOST, PLAYER] kick <player>

                    // [PLAYER] refresh
                    if (ctx == "refresh")
                    {
                        // Ignore if they do not exist
                        if (player == null)
                            break;

                        // Refresh the display, if possible
                        await connection.RefreshAsync(server);
                        return;
                    }

                    // [HOST, PLAYER] back
                    if (ctx == "back")
                    {
                        // if they are already in the server
                        if (player == null)
                            break;

                        // if they are not the host
                        if (!player.Host)
                        {
                            string notice = $"[To {user.Username}] Only the host may exit the server configuration.";
                            AppendToConsole(server, notice);
                            break;
                        }

                        connection.State = GameState.Waiting;
                        connection.Frequency = 0;

                        RefreshConsole(server);
                        RefreshConsoleHeader(server);
                        break;
                    }

                    // [HOST, PLAYER] title <value>
                    if (ctx.StartsWith("title"))
                    {
                        var notice = "";

                        // if they are already in the server
                        if (player == null)
                            break;

                        // if they are not the host
                        if (!player.Host)
                        {
                            notice = $"[To {user.Username}] Only the host may edit the server's title.";
                            AppendToConsole(server, notice);
                            break;
                        }

                        var reader = new StringReader(ctx);
                        reader.Skip(5);
                        reader.SkipWhiteSpace();

                        // Limit title length to 42 (32 for username limit, with an additional 10 characters)
                        string title = reader.GetRemaining();

                        if (!Check.NotNull(title))
                            notice = $"[To {user.Username}] A title cannot be empty or consist of only whitespace characters.";
                        else if (Format.IsSensitive(title))
                            notice = $"[To {user.Username}] A title cannot contain any Markdown sequence characters.";
                        else if (title.Length > ServerProperties.MaxNameLength)
                            notice = $"[To {user.Username}] A title must be less than or equal to {ServerProperties.MaxNameLength} characters in size.";
                        else
                        {
                            server.Config.Name = title;
                            notice = $"[Console] The title of this server has been renamed to \"{server.Config.Name}\".";
                            RefreshServerConfig(server);
                            RefreshConsoleHeader(server);
                            RefreshConsole(server);
                        }

                        AppendToConsole(server, notice);
                        break;
                    }

                    // [HOST, PLAYER] game <value>
                    if (ctx.StartsWith("game"))
                    {

                        // If they are already in the server
                        if (player == null)
                            break;

                        // If they are not the host
                        if (!player.Host)
                        {
                            string notice = $"[To {user.Username}] Only the host may edit the server's game mode.";
                            AppendToConsole(server, notice);
                            break;
                        }

                        var reader = new StringReader(ctx);
                        reader.Skip(4);
                        reader.SkipWhiteSpace();

                        string game = reader.ReadString();

                        if (Check.NotNull(game))
                        {
                            server.Config.GameId = game;

                            if (server.Config.IsValidGame())
                            {
                                string notice = $"[Console] The game mode has been set to '{server.Config.GameId}'.";
                                AppendToConsole(server, notice);
                                RefreshServerConfig(server);

                                server.Config.LoadGame();
                                RefreshGameConfig(server);
                                RefreshConsoleHeader(server);
                            }
                            else
                            {
                                string notice = $"[To {user.Username}] An unknown game mode was specified.";
                                AppendToConsole(server, notice);
                                server.Config.GameId = "Trivia";
                            }
                        }

                        break;
                    }

                    // [HOST, PLAYER] privacy <value>
                    if (ctx.StartsWith("privacy"))
                    {
                        var notice = "";

                        // if they are already in the server
                        if (player == null)
                            break;

                        // if they are not the host
                        if (!player.Host)
                        {
                            notice = $"[To {user.Username}] Only the host may edit the server's privacy.";
                            AppendToConsole(server, notice);
                            break;
                        }

                        var reader = new StringReader(ctx);
                        reader.Skip(7);
                        reader.SkipWhiteSpace();

                        string remainder = reader.GetRemaining();

                        if (Enum.TryParse(remainder, true, out Privacy privacy))
                        {
                            server.Config.Privacy = privacy;
                            notice = $"[Console] The privacy of this server has been set to {server.Config.Privacy.ToString()}.";
                            RefreshServerConfig(server);
                        }
                        else
                        {
                            notice = $"[To {user.Username}] Unable to parse the specified privacy.";
                        }

                        AppendToConsole(server, notice);
                        break;
                    }

                    // if no other commands are valid, attempt to parse a custom configuration
                    // otherwise, just return.
                    // [HOST, PLAYER] CUSTOM <value>
                    if (server.Config.IsValidGame())
                    {
                        GameBase game = server.Config.LoadGame();

                        foreach(GameOption option in game.Options)
                        {
                            if (ctx.StartsWith(option.Id))
                            {
                                var notice = "";

                                if (player == null)
                                    break;

                                if (!player.Host)
                                {
                                    notice = $"[To {user.Username}] Only the host may edit configuration values.";
                                    AppendToConsole(server, notice);
                                    break;
                                }

                                var reader = new StringReader(ctx);
                                reader.Skip(option.Id.Length);
                                reader.SkipWhiteSpace();

                                string remainder = reader.GetRemaining();

                                // Using the remainder of the input given, attempt to parse the specified type
                                /*
                                if (option.ValueType.IsEnum)
                                {
                                    if (Enum.TryParse(option.ValueType, remainder, true, out object result))
                                        server.Config.SetOption(option.Id, result);
                                }
                                */

                                // I don't know if the type parser is efficient or not, confirm later on
                                if (TypeParser.TryParse(option.ValueType, remainder, out object result))
                                {
                                    server.Config.SetOption(option.Id, result);
                                    notice = $"[To {user.Username}] Updated \"{option.Name}\" to the specified value.";
                                    RefreshGameConfig(server);
                                }
                                else
                                {
                                    notice = $"[To {user.Username}] Unable to parse the specified value given.";
                                }

                                AppendToConsole(server, notice);
                                break;
                            }
                        }
                    }

                    break;

                // This represents the spectator control panel (INCOMPLETE)
                case GameState.Watching:
                    // [HOST, PLAYER] end
                    if (ctx == "end")
                    {
                        // if they are not in the server
                        if (player == null)
                            break;

                        // if they are the host, force end the current session

                        // otherwise, send a note stating that they cannot force end a session unless
                        // they are a host

                        break;
                        //break;
                    }

                    // [PLAYER] back
                    if (ctx == "back")
                    {
                        // if they are not in the server
                        if (player == null)
                            break;

                        // if they are the host, automatically call

                        // otherwise, begin calling the vote to switch over to spectator mode.

                        break;
                        //break;
                    }

                    break;

                // This represents a custom display channel for a server connection
                // If they are currently in an active session, handle the specified inputs instead for the specified display channel
                case GameState.Playing:
                    // Get the display channel that the server connection is referencing
                    DisplayChannel display = server.GetDisplayChannel(connection.Frequency);

                    // Iterate through all inputs specified
                    foreach (IInput input in display.Inputs)
                    {
                        string rawCtx = ctx;

                        // If the following input type is a TextInput
                        if (input is TextInput tInput)
                        {
                            // Check to see if it's case sensitive
                            if (!tInput.CaseSensitive)
                                rawCtx = ctx.ToLower();
                        }

                        // Parse the following input
                        InputResult result = input.TryParse(rawCtx);

                        if (result.IsSuccess)
                        {
                            // Check criterion, if any
                            if (result.Input.Criterion != null)
                                if (!result.Input.Criterion(user, connection, server))
                                    break;

                            if (result.Input.OnExecute == null)
                                throw new Exception("Expected a function for the following input but returned null");
                            
                            // Execute the specified method for the matched input
                            result.Input.OnExecute(new InputContext(user, connection, server, result));

                            // If the existing player no longer exists in the server
                            if (player != null)
                            {
                                if (server.GetPlayer(user.Id) == null)
                                {
                                    // Remove the player that was reserved
                                    ReservedUsers.Remove(user.Id);

                                    // If the player that was removed was the only player, destroy the server
                                    if (server.Players.Count == 0)
                                    {
                                        await DestroyServerAsync(server);
                                        return;
                                    }

                                    // If the player that was removed was the host, set a new host
                                    if (server.Host == null)
                                        server.Players.OrderBy(x => x.JoinedAt).First().Host = true;
                                }
                            }
                            // Likewise, if there wasn't a player before and they were added, add them to the reserves
                            else if (server.GetPlayer(user.Id) != null)
                                ReservedUsers[user.Id] = server.Id;
                            
                            // Make a check if the server is allowed to update
                            allowUpdate = result.Input.UpdateOnExecute;
                        }

                        // Stop reading inputs once one is successful
                        break;
                    }

                    break;
            }
            
            // If the user isn't a part of the server, leave their message and add one to the counter
            if (server.GetPlayer(user.Id) == null)
            {
                if (connection.RefreshCounter > 0) // If the refresh counter is greater than 0
                {
                    
                    connection.CurrentMessageCounter++;
                    Console.WriteLine($"{connection.CurrentMessageCounter}/{connection.RefreshCounter} refreshes called");
                    if (connection.CurrentMessageCounter >= connection.RefreshCounter)
                    {
                        await connection.RefreshAsync(server);
                    }
                }

                // If the game server can update, do so
                if (allowUpdate)
                    await server.UpdateAsync();

                return;
            }

            // Check to see if the bot is allowed to delete messages for the server connection
            if (connection.CouldDeleteMessages)
                await EnsureDeleteAsync(connection);

            // If the bot is allowed to delete messages in this server connection
            if (connection.CanDeleteMessages)
                await message.DeleteAsync();
            else if (connection.RefreshCounter > 0) // If the refresh counter is greater than 0
            {
                Console.WriteLine($"{connection.CurrentMessageCounter}/{connection.RefreshCounter} refreshes called");
                connection.CurrentMessageCounter++;

                if (connection.CurrentMessageCounter >= connection.RefreshCounter)
                {
                    await connection.RefreshAsync(server);
                }
            }

            // If the game server can update, do so
            if (allowUpdate)
                await server.UpdateAsync();
        }

        // Checks to see if it can delete messages for the specified server connection
        private async Task EnsureDeleteAsync(ServerConnection connection)
        {
            if (connection.GuildId.HasValue)
            {
                IGuild guild = _client.GetGuild(connection.GuildId.Value);

                if (guild != null)
                {
                    IGuildUser bot = await guild.GetCurrentUserAsync();
                    connection.CanDeleteMessages = bot?.GuildPermissions.ManageMessages ?? false;
                }
            }
            else
            {
                connection.CanDeleteMessages = connection.Type == ConnectionType.Direct;
            }

            connection.CouldDeleteMessages = false;

            Console.WriteLine($"Can delete messages? {connection.CanDeleteMessages}");
        }

        public async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            // check to see where the message was deleted

            // If the deletion was in a reserved channel, get the server that contains the channel
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                GameServer server = Servers[ReservedChannels[channel.Id]];
                ServerConnection connection = server
                    .Connections
                    .First(x => x.MessageId == message.Id);
                
                // Send a new message to that channel's connection to refresh it
                connection.InternalMessage = await connection.Channel
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