using Discord;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Net;
using Orikivo.Framework;
using Format = Orikivo.Format;
using Orikivo.Text;

namespace Arcadia.Multiplayer
{
    public class GameManager
    {
        public static readonly TimeSpan ServerIdleTimeout = TimeSpan.FromMinutes(5);
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
            _client.LatencyUpdated += HandleIdleServers;
        }

        public IReadOnlyDictionary<string, GameInfo> Games { get; set; }
        public string DefaultGameId { get; set; }
        public Dictionary<string, GameServer> Servers { get; }

        /// <summary>
        /// Represents a collection of all channels that are currently bound to a <see cref="GameServer"/>.
        /// </summary>
        internal Dictionary<ulong, string> ReservedChannels { get; }

        /// <summary>
        /// Represents a collection of all users that are currently bound to a <see cref="GameServer"/>.
        /// </summary>
        internal Dictionary<ulong, string> ReservedUsers { get; }

        private async Task HandleIdleServers(int previous, int current)
        {
            foreach (GameServer server in Servers.Values.Where(s => DateTime.UtcNow - s.LastUpdated >= ServerIdleTimeout))
            {
                await DestroyServerAsync(server).ConfigureAwait(false);
            }
        }

        public GameServer GetServerFor(ulong userId)
        {
            if (ReservedUsers.ContainsKey(userId))
                return Servers[ReservedUsers[userId]];

            return null;
        }

        public IEnumerable<GameServer> GetServersFor(ulong userId, ulong guildId = 0)
        {
            return Servers.Values.Where(x => MeetsServerCriterion(x, userId, guildId));
        }

        private IEnumerable<GameServer> GetOpenServersFor(ulong userId, ulong guildId = 0, string gameId = null)
        {
            return Servers.Values.Where(x =>
                MeetsServerCriterion(x, userId, guildId)
                && !x.IsFull
                && (string.IsNullOrWhiteSpace(gameId) || x.GameId == gameId));
        }

        private static bool MeetsServerCriterion(GameServer server, ulong userId, ulong guildId)
        {
            return server.Privacy switch
            {
                Privacy.Public => true,
                Privacy.Local => server.Connections.Any(x => x.GuildId == guildId),
                _ => server.Invites.Any(x => x.UserId == userId)
            };
        }

        private IEnumerable<GameServer> GetOpenServers()
            => Servers.Values.Where(x => x.Privacy == Privacy.Public && !x.IsFull);

        public IEnumerable<GameServer> GetPublicServers()
            => Servers.Values.Where(x => x.Privacy == Privacy.Public);

        public string GetRandomServer()
        {
            return Randomizer.ChooseOrDefault(GetOpenServers(), x=> !x.IsFull)?.Id;
        }

        public string GetRandomServerFor(ulong userId, ulong guildId = 0)
        {
            return Randomizer.ChooseOrDefault(GetOpenServersFor(userId, guildId)).Id;
        }

        public string GetRandomServer(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Randomizer.Choose(GetPublicServers(), x => x.GameId == gameId && !x.IsFull)?.Id;
        }

        public string GetRandomServerFor(ulong userId, string gameId, ulong guildId = 0)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Randomizer.ChooseOrDefault(GetOpenServersFor(userId, guildId, gameId))?.Id;
        }

        // This attempts to join an existing server, if one is present
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
                    await channel.SendMessageAsync(Format.Warning("You have already joined this server elsewhere."));
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

            switch (server.Privacy)
            {
                case Privacy.Public:
                case Privacy.Unlisted:
                    await server.AddPlayerAsync(user);
                    await server.AddConnectionAsync(channel);
                    break;

                case Privacy.Local:
                    if (server.Connections.All(x => x.ChannelId != channel.Id))
                    {
                        await channel.SendMessageAsync(Format.Warning("You can only join this server from where it was initialized."));
                        break;
                    }

                    await server.AddPlayerAsync(user);
                    break;

                default:
                    throw new Exception("Unknown privacy state");
            }
        }

        public GameDetails DetailsOf(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Games[gameId].Details;
        }

        public GameInfo GetGame(string gameId)
        {
            if (!Games.ContainsKey(gameId))
                return null;

            return Games[gameId];
        }

        private void RefreshConsoleHeader(GameServer server)
        {
            GameDetails details = DetailsOf(server.GameId);

            string playerCounter = $"**{server.Players.Count:##,0}** {Format.TryPluralize("player", server.Players.Count)}";

            if (details != null)
            {
                playerCounter = $"**{server.Players.Count:##,0}**/**{details.PlayerLimit:##,0}** {Format.TryPluralize("player", details.PlayerLimit)}";
            }

            server
                .GetBroadcast(GameState.Waiting)
                .GetComponent(LobbyVars.Header)
                .Draw(server.Name, server.Id, Format.Title(details?.Name ?? server.GameId, details?.Icon), playerCounter);
        }

        // this starts a new base game server
        public async Task CreateServerAsync(IUser user, IMessageChannel channel, string gameId = null, Privacy privacy = Privacy.Public)
        {
            if (user.IsBot)
                return;

            ServerProperties properties = ServerProperties.GetDefault(user.Username);

            gameId = gameId?.ToLower();

            if (!string.IsNullOrWhiteSpace(gameId) && Games.ContainsKey(gameId))
                properties.GameId = gameId;

            properties.Privacy = privacy;
            var server = await GameServer.CreateAsync(this, user, channel, properties);

            foreach (ServerConnection connection in server.Connections)
                await SetDeleteStateAsync(connection);
        }

        internal void EndSession(GameSession session)
        {
            GameResult result = session.Game.OnGameFinish(session);
            result.Apply(_container, _container.Data.GetOrAssignBonusGame(this));

            foreach (ArcadeUser user in session.Server.Players.Select(delegate(Player player)
            {
                _container.Users.TryGet(player.User.Id, out ArcadeUser account);
                return account;
            }).Where(x => x != null))
            {
                ChallengeHelper.UpdateChallengeProgress(user, result);
            }
        }

        // Destroys the session of a game server
        internal async Task DestroySessionAsync(GameServer server)
        {
            if (server.Session != null)
            {
                server.DestroyCurrentSession();
                AddConsoleText(server, "[Console] The current session has been destroyed.");
            }
        }

        // destroys the game server accordingly
        internal async Task DestroyServerAsync(GameServer server)
        {
            // Override the display to specify that the server was destroyed
            server.GetBroadcast(GameState.Waiting).Content.ValueOverride = $"> ⚠️ **{server.Name}** has been shut down.\n> Sorry about the inconvenience.";

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
            server.Destroyed = true;
        }

        public async Task OnChannelDestroyed(SocketChannel channel)
        {
            if (ReservedChannels.ContainsKey(channel.Id))
                await Servers[ReservedChannels[channel.Id]].RemoveConnectionAsync(channel.Id);
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

                DisplayBroadcast display = server.GetBroadcast(connection.Frequency);

                foreach (IInput input in display.Inputs)
                {
                    if (!(input is ReactionInput))
                        continue;

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

        // It might be better to make the private methods used in the game server, and make then internal
        // Refreshes the server's configuration details
        private void RefreshServerConfig(GameServer server)
        {
            GameDetails details = DetailsOf(server.GameId);
            string gameName = Check.NotNull(details?.Name) ? details?.Name : server.GameId;

            server.GetBroadcast(GameState.Editing)
                .Content.GetComponent("config")
                .Draw(server.Name, server.Privacy, gameName, server.AllowSpectators, server.AllowInvites, server.AllowChat);
        }

        private void RefreshGameConfig(GameServer server)
        {
            DisplayContent editing = server.GetBroadcast(GameState.Editing).Content;

            if (!Games.ContainsKey(server.GameId))
                return;

            if (!Check.NotNullOrEmpty(server.Options))
            {
                editing["game_config"].Active = false;
                return;
            }

            editing["game_config"].Active = true;
            editing["game_config"].Draw(
                server.Options.Select(x => $"{x.Name}: **{x.Value}**"),
                DetailsOf(server.GameId).Name);
        }

        private static void RefreshConsole(GameServer server)
        {
            server.GetBroadcast(GameState.Editing).Content.GetComponent(LobbyVars.Console).Draw(server.Name);
            server.GetBroadcast(GameState.Waiting).Content.GetComponent(LobbyVars.Console).Draw();
        }

        // Appends the specified message to all of the reserved console components and updates accordingly
        private static void AddConsoleText(GameServer server, string message, bool draw = true)
        {
            DisplayContent waiting = server.GetBroadcast(GameState.Waiting).Content;
            DisplayContent editing = server.GetBroadcast(GameState.Editing).Content;

            waiting.GetGroup(LobbyVars.Console).Append(message);
            editing.GetGroup(LobbyVars.Console).Append(message);

            if (!draw)
                return;

            editing.GetComponent(LobbyVars.Console).Draw(server.Name);
            waiting.GetComponent(LobbyVars.Console).Draw();
        }

        private static string WritePlayerInfo(Player player)
        {
            var info = new StringBuilder();

            info.Append(player.User.Username);

            if (player.IsHost)
                info.Append(" [Host]");

            if (player.Playing)
                info.Append(" - Playing");

            return info.ToString();
        }

        private static string WriteConnectionInfo(ServerConnection connection)
        {
            var info = new StringBuilder();

            //if (connection.GuildId.HasValue)
            //    info.Append($"{connection.GuildId}/");

            info.Append($"{connection.ChannelId}");
            info.Append($" - {connection.State} ({connection.Frequency})");

            return info.ToString();
        }

        // TODO: Reduce method complexity and reduce Logger usage (or limit it to the debug build only)
        public async Task OnMessageReceived(SocketMessage message)
        {
            bool allowUpdate = true;
            IUser user = message.Author;

            // Ignore all bots
            if (user.IsBot)
                return;

            if (!ReservedChannels.ContainsKey(message.Channel.Id))
            {
                // Logger.Debug("Message channel not reserved to server");
                return;
            }

            // Ignore if the user is already reserved but the servers references don't match
            if (ReservedUsers.ContainsKey(user.Id))
            {
                if (ReservedChannels[message.Channel.Id] != ReservedUsers[user.Id])
                {
                    // Logger.Debug("Reserved channel does not match reserved user");
                    return;
                }
            }

            // Logger.Debug("Reserve comparison success");

            // Throw an error if the reserved reference points to an empty server
            if (!Servers.ContainsKey(ReservedChannels[message.Channel.Id]))
            {
                Logger.Debug("Unable to find the requested server from the specified channel");
                throw new Exception("Unable to find the requested server for the specified user");
            }

            GameServer server = Servers[ReservedChannels[message.Channel.Id]];

            // Throw an error if the specified channel saved in the reserves doesn't exist on this server
            if (!server.HasConnection(message.Channel.Id))
            {
                Logger.Debug("Unable to retrieve the server connection for the specified channel");
                throw new Exception("Unable to retrieve the server connection for the specified channel");
            }

            // Logger.Debug("Ensure connection success");

            Player player = server.GetPlayer(user.Id);
            ServerConnection connection = server.GetConnection(message.Channel.Id);

            // Extract only the content of the message
            string ctx = message.Content;
            var reader = new StringReader(ctx);

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

                // Logger.Debug("Session ensure success");
            }

            allowUpdate = player != null;

            if (!connection.BlockInput && (!server.Session?.BlockInput ?? true))
            {
                // Load all of the display channels for the specified game states
                DisplayContent waiting = server.GetBroadcast(GameState.Waiting).Content;
                DisplayContent editing = server.GetBroadcast(GameState.Editing).Content;

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
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may start the game.");
                                break;
                            }

                            allowUpdate = !await server.StartGameAsync();
                            break;
                        }

                        // join
                        if (ctx == "join")
                        {
                            // Ignore if they already exist
                            if (player != null)
                                break;

                            allowUpdate = false;
                            await server.AddPlayerAsync(user);
                            break;
                        }

                        // [PLAYER] leave
                        if (ctx == "leave")
                        {
                            // Ignore if they do not exist
                            if (player == null)
                                break;

                            allowUpdate = false;
                            await server.RemovePlayerAsync(user.Id);
                            break;
                        }

                        // [HOST, PLAYER] invite <user>
                        /*
                        if (ctx == "invite")
                        {
                            if (player == null)
                                break;

                            allowUpdate = false;
                            // [Console] Sent an invite to USERNAME.
                            // [Console] Unable to send an invite to USERNAME.
                        }
                        */

                        // [PLAYER] refresh
                        if (ctx == "refresh")
                        {
                            // Ignore if the player does not exist
                            if (player == null)
                                break;

                            allowUpdate = false;
                            await connection.RefreshAsync(true);
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
                            AddConsoleText(server, buffer);
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
                            if (player.IsHost)
                                buffer = $"Connections ({server.Connections.Count}): {string.Join(", ", server.Connections.Select(WriteConnectionInfo))}";

                            AddConsoleText(server, buffer);
                            break;
                        }

                        // [HOST, PLAYER] config
                        if (ctx == "config")
                        {
                            // Ignore if they do not exist
                            if (player == null)
                                break;

                            var notice = "";

                            // If they are not the host
                            if (!player.IsHost)
                                notice = $"[To {user.Username}] Only the host can edit the server config.";

                            // If there is an existing connection already in the editing state
                            else if (server.Connections.Any(x => x.State == GameState.Editing))
                                notice = $"[Console] There is already a connection that is editing the configuration.";

                            // Otherwise, set this specific server connection to editing mode.
                            else
                            {
                                connection.State = GameState.Editing;
                                connection.Frequency = 1;
                                RefreshGameConfig(server);
                                RefreshServerConfig(server);
                                editing["console"].Draw(server.Name);
                                break;
                            }

                            AddConsoleText(server, notice);
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
                                AddConsoleText(server, buffer);
                                break;
                            }

                            if (!server.AllowSpectators)
                            {
                                buffer = "[Console] Spectating is disabled on this server.";
                                AddConsoleText(server, buffer);
                                break;
                            }

                            if (!server.Session.Game.Details.CanSpectate)
                            {
                                buffer = "[Console] The current session does not support spectating.";
                                AddConsoleText(server, buffer);
                                break;
                            }

                            // if they are the host, automatically start it.
                            if (player.IsHost)
                            {
                                connection.State = GameState.Watching;
                                connection.Frequency = 2;
                                break;
                            }

                            connection.State = GameState.Watching;
                            connection.Frequency = 2;
                            break;

                            // TODO: start up a spectator vote session, and if > 67% of players agree, start spectating
                            // buffer = $"[Console] Multiple spectator s";
                            //AddConsoleText(server, buffer);
                            //break;
                        }

                        if (ctx.StartsWith("say"))
                        {
                            if (player == null)
                                break;

                            if (!server.AllowChat)
                                break;

                            if (DateTime.UtcNow - player.LastSpoke < TimeSpan.FromSeconds(10))
                                break;

                            reader.Skip(3);
                            reader.SkipWhiteSpace();

                            string content = reader.GetRemaining();

                            if (!Check.NotNull(content))
                                break;

                            if (content.Length > 48)
                                content = content.Substring(0, 48);

                            if (Format.IsSensitive(content))
                                break;

                            AddConsoleText(server, $"[{user.Username}] {content}");
                            player.LastSpoke = DateTime.UtcNow;
                            break;
                        }

                        if (ctx.StartsWith("me"))
                        {
                            if (player == null)
                                break;

                            if (!server.AllowChat)
                                break;

                            if (DateTime.UtcNow - player.LastSpoke < TimeSpan.FromSeconds(10))
                                break;

                            reader.Skip(2);
                            reader.SkipWhiteSpace();

                            string content = reader.GetRemaining();

                            if (!Check.NotNull(content))
                                break;

                            if (content.Length > 48)
                                content = content.Substring(0, 48);

                            if (Format.IsSensitive(content))
                                break;

                            AddConsoleText(server, $"{user.Username} {content}");
                            player.LastSpoke = DateTime.UtcNow;
                            break;
                        }

                        if (ctx.StartsWith("invite"))
                        {
                            if (player == null)
                                break;

                            if (!server.AllowInvites)
                                break;

                            if (DateTime.UtcNow - player.LastInviteSent < TimeSpan.FromSeconds(15))
                                break;

                            reader.Skip(6);
                            reader.SkipWhiteSpace();

                            if (!ulong.TryParse(reader.GetRemaining(), out ulong targetId))
                                break;

                            Logger.Debug($"parse success {reader.GetRemaining()}");
                            allowUpdate = true;

                            if (server.Invites.Any(x => x.UserId == targetId))
                            {
                                AddConsoleText(server, $"[To {user.Username}] The specified user already has an active invitation.");
                                break;
                            }

                            IUser target = _client.GetUser(targetId);

                            if (target == null)
                            {
                                AddConsoleText(server, $"[Console] Unable to find the specified user.");
                                break;
                            }

                            var invite = new ServerInvite(targetId, null);
                            server.Invites.Add(invite);
                            AddConsoleText(server, $"[Console] Sent a server invite to {target.Username}.");
                            player.LastInviteSent = DateTime.UtcNow;

                            try
                            {
                                await target.SendMessageAsync(ServerInvite.Write(invite, server)).ConfigureAwait(false);
                            }
                            catch (HttpException error)
                            {
                                if (error.DiscordCode.GetValueOrDefault(0) != 50007)
                                    throw;

                                Logger.Debug($"[{Format.Time(DateTime.UtcNow)}] Unable to send message to user {target.Id} as their direct message channel is disabled");
                            }

                            break;
                        }

                        // [HOST, PLAYER] kick <player>
                        if (ctx.StartsWith("kick"))
                        {
                            // If they are already in the server
                            if (player == null)
                                break;

                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may kick other players.");
                                break;
                            }

                            reader.Skip(4);
                            reader.SkipWhiteSpace();

                            string toKick = reader.GetRemaining();

                            if (!Check.NotNull(toKick))
                            {
                                AddConsoleText(server, $"[Console] A player must be specified.");
                            }

                            if (ulong.TryParse(toKick, out ulong kickId))
                            {
                                if (server.Players.All(x => x.User.Id != kickId))
                                {
                                    AddConsoleText(server, $"[Console] Unable to find the specified player.");
                                    break;
                                }
                            }

                            if (server.Players.All(x => x.User.Username != toKick))
                            {
                                AddConsoleText(server, $"[Console] Unable to find the specified player.");
                                break;
                            }

                            if (server.Players.Count(x => x.User.Username == toKick) > 1)
                            {
                                AddConsoleText(server, $"[Console] There is more than one player with the specified username.");
                                break;
                            }

                            Player target = server.Players.First(x => x.User.Username == toKick);
                            // TODO: Implement reasons

                            await server.RemovePlayerAsync(target.User.Id);
                        }

                        break;

                    // This represents the configuration panel
                    case GameState.Editing:
                        // join
                        if (ctx == "join")
                        {
                            // Ignore if they already exist
                            if (player == null)
                                break;

                            allowUpdate = false;
                            await server.AddPlayerAsync(user);
                            break;
                        }

                        // [PLAYER] leave
                        if (ctx == "leave")
                        {
                            // Ignore if they do not exist
                            if (player == null)
                                break;

                            allowUpdate = false;
                            await server.RemovePlayerAsync(user.Id);
                            break;
                        }

                        // [PLAYER] players
                        if (ctx == "players") // TODO: Implement cooldown
                        {
                            // Ignore if they do not exist
                            if (player == null)
                                break;

                            // This has to handle in the case of too many players being in a server at once
                            string buffer = $"Players ({server.Players.Count}): {string.Join(", ", server.Players.Select(WritePlayerInfo))}";
                            AddConsoleText(server, buffer);
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
                            if (player.IsHost)
                                buffer = $"Connections ({server.Connections.Count}): {string.Join(", ", server.Connections.Select(WriteConnectionInfo))}";

                            AddConsoleText(server, buffer);
                            break;
                        }

                        // [HOST, PLAYER] kick <player>

                        // [PLAYER] refresh
                        if (ctx == "refresh")
                        {
                            // Ignore if they do not exist
                            if (player == null)
                                break;

                            await connection.RefreshAsync(true);
                            return;
                        }

                        // [HOST, PLAYER] back
                        if (ctx == "back")
                        {
                            // if they are already in the server
                            if (player == null)
                                break;

                            // if they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may exit the server configuration.");
                                break;
                            }

                            connection.State = GameState.Waiting;
                            connection.Frequency = 0;

                            RefreshConsole(server);
                            RefreshConsoleHeader(server);
                            break;
                        }

                        if (ctx == "togglespectate")
                        {
                            // if they are already in the server
                            if (player == null)
                                break;

                            // if they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may toggle this setting.");
                                break;
                            }

                            allowUpdate = !await server.ToggleSpectateAsync();
                            break;
                        }

                        if (ctx == "togglechat")
                        {
                            // if they are already in the server
                            if (player == null)
                                break;

                            // if they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may toggle this setting.");
                                break;
                            }

                            allowUpdate = false;
                            await server.ToggleChatAsync();
                            break;
                        }

                        if (ctx == "toggleinvite")
                        {
                            // if they are already in the server
                            if (player == null)
                                break;

                            // if they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may toggle this setting.");
                                break;
                            }

                            allowUpdate = false;
                            await server.ToggleInviteAsync();
                            break;
                        }

                        // [HOST, PLAYER] kick <player>
                        if (ctx.StartsWith("kick"))
                        {
                            // If they are already in the server
                            if (player == null)
                                break;

                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may kick other players.");
                                break;
                            }

                            reader.Skip(4);
                            reader.SkipWhiteSpace();

                            string toKick = reader.GetRemaining();

                            if (!Check.NotNull(toKick))
                            {
                                AddConsoleText(server, $"[Console] A player must be specified.");
                            }

                            if (ulong.TryParse(toKick, out ulong kickId))
                            {
                                if (server.Players.All(x => x.User.Id != kickId))
                                {
                                    AddConsoleText(server, $"[Console] Unable to find the specified player.");
                                    break;
                                }
                            }

                            if (server.Players.All(x => x.User.Username != toKick))
                            {
                                AddConsoleText(server, $"[Console] Unable to find the specified player.");
                                break;
                            }

                            if (server.Players.Count(x => x.User.Username == toKick) > 1)
                            {
                                AddConsoleText(server, $"[Console] There is more than one player with the specified username.");
                                break;
                            }

                            Player target = server.Players.First(x => x.User.Username == toKick);
                            // TODO: Implement reasons

                            allowUpdate = !await server.RemovePlayerAsync(target.User.Id);
                        }

                        // [HOST, PLAYER] title <value>
                        if (ctx.StartsWith("name"))
                        {
                            // If they are already in the server
                            if (player == null)
                                break;

                            // If they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may edit the server's name.");
                                break;
                            }

                            reader.Skip(4);
                            reader.SkipWhiteSpace();

                            // Limit title length to 42 (32 for username limit, with an additional 10 characters)
                            string name = reader.GetRemaining();
                            allowUpdate = !await server.UpdateNameAsync(name);
                            break;
                        }

                        // [HOST, PLAYER] game <value>
                        if (ctx.StartsWith("game"))
                        {
                            // If they are already in the server
                            if (player == null)
                                break;

                            // If they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may edit the server's game mode.");
                                break;
                            }

                            reader.Skip(4);
                            reader.SkipWhiteSpace();

                            string game = reader.ReadString();
                            allowUpdate = !await server.UpdateGameAsync(game);
                            break;
                        }

                        // [HOST, PLAYER] privacy <value>
                        if (ctx.StartsWith("privacy"))
                        {
                            // if they are already in the server
                            if (player == null)
                                break;

                            // if they are not the host
                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may edit the server's privacy.");
                                break;
                            }

                            reader.Skip(7);
                            reader.SkipWhiteSpace();

                            string remainder = reader.GetRemaining();
                            bool isValid = Enum.TryParse(remainder, true, out Privacy privacy);

                            if (!isValid)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Unable to parse the specified privacy.");
                                break;
                            }

                            allowUpdate = !await server.UpdatePrivacyAsync(privacy);
                            break;
                        }

                        // if no other commands are valid, attempt to parse a custom configuration
                        // otherwise, just return.
                        // [HOST, PLAYER] CUSTOM <value>
                        if (Games.ContainsKey(server.GameId))
                        {
                            if (player == null)
                                break;

                            if (!player.IsHost)
                            {
                                AddConsoleText(server, $"[To {user.Username}] Only the host may edit configuration values.");
                                break;
                            }

                            string id = reader.ReadUnquotedString();

                            if (server.Options.All(x => x.Id != id))
                                break;

                            if (!reader.Contains(' '))
                            {
                                AddConsoleText(server, "[Console] A value was not specified for the option.");
                                break;
                            }

                            reader.SkipWhiteSpace();

                            string remainder = reader.GetRemaining();
                            Logger.Debug($"{id} => {remainder}");
                            allowUpdate = !await server.SetOptionAsync(id, remainder);
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
                            connection.State = GameState.Waiting;
                            connection.Frequency = 0;
                            break;
                        }

                        break;

                    // This represents a custom display channel for a server connection
                    // If they are currently in an active session, handle the specified inputs instead for the specified display channel
                    case GameState.Playing:
                        if (server.Session == null)
                            throw new Exception("Expected server to have an active game session");

                        foreach (IInput input in connection.GetAvailableInputs())
                        {
                            // Ignore if a player is required and their data is not in the session
                            if (input.RequirePlayer && server.Session.DataOf(user.Id) == null)
                                continue;

                            // Ignore if the input type is not a text input
                            if (!(input is TextInput))
                                continue;

                            InputResult result = input.TryParse(ctx);

                            // Ignore if the input is not successful
                            if (!result.IsSuccess)
                                continue;

                            // Check criterion, if any
                            if (result.Input.Criterion != null)
                            {
                                if (!result.Input.Criterion(user, connection, server))
                                    break;
                            }

                            if (result.Input.OnExecute == null)
                                throw new Exception("Expected a function for the following input but returned null");

                            try
                            {
                                result.Input.OnExecute(new InputContext(user, connection, server, result));

                                // If the server was destroyed by this input, return
                                if (server.Destroyed)
                                    return;

                                // Otherwise, make a check if the server is allowed to update
                                allowUpdate = result.Input.UpdateOnExecute;

                                Logger.Debug($"Handled input successfully");
                                break;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                server.DestroyCurrentSession();
                                AddConsoleText(server,
                                    $"[Console] An exception was thrown while handling an input.");
                                allowUpdate = true;
                                break;
                            }
                        }

                        break;
                }

                Logger.Debug("Handled input");
            }
            else
            {
                allowUpdate = false;
            }

            if (server.Destroyed)
                return;

            // If the bot is allowed to delete messages in this server connection
            if (connection.DeleteMessages && server.GetPlayer(user.Id) != null)
            {
                Logger.Debug("Attempt delete message");
                try
                {
                    if (!await message.TryDeleteAsync(new RequestOptions
                    {
                        RetryMode = RetryMode.RetryTimeouts | RetryMode.Retry502
                    }))
                    {
                        connection.DeleteMessages = false;
                        Logger.Debug("Revoked delete permission");
                    }
                }
                catch (RateLimitedException)
                {
                    if (connection.RefreshCounter > 0)
                    {
                        connection.CurrentMessageCounter++;
                        Logger.Debug($"{connection.CurrentMessageCounter}/{connection.RefreshCounter} refreshes called");
                        if (connection.CurrentMessageCounter >= connection.RefreshCounter)
                        {
                            Logger.Debug("Replacing connection content");
                            await connection.RefreshAsync();
                        }
                    }
                }
            }
            else if (connection.RefreshCounter > 0) // If the refresh counter is greater than 0
            {
                connection.CurrentMessageCounter++;
                Logger.Debug($"{connection.CurrentMessageCounter}/{connection.RefreshCounter} refreshes called");

                if (connection.CurrentMessageCounter >= connection.RefreshCounter)
                {
                    Logger.Debug("Replacing connection content");
                    await connection.RefreshAsync();
                }
            }

            if (allowUpdate)
            {
                await server.UpdateAsync();
                Logger.Debug("Allow update success");
            }
        }

        // Checks to see if it can delete messages for the specified server connection
        internal async Task SetDeleteStateAsync(ServerConnection connection)
        {
            if (connection.GuildId.HasValue)
            {
                IGuild guild = _client.GetGuild(connection.GuildId.Value);

                if (guild != null)
                {
                    IGuildUser bot = await guild.GetCurrentUserAsync();
                    connection.DeleteMessages = bot?.GuildPermissions.ManageMessages ?? false;
                }
            }
            else
            {
                connection.DeleteMessages = connection.Type == ConnectionType.Direct;
            }

            Logger.Debug($"Ensured deletion state as {connection.DeleteMessages}");
        }

        public async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            // check to see where the message was deleted

            // If the deletion was in a reserved channel, get the server that contains the channel
            if (ReservedChannels.ContainsKey(channel.Id))
            {
                GameServer server = Servers[ReservedChannels[channel.Id]];
                ServerConnection connection = server.GetConnection(channel.Id);

                if (connection.MessageId == message.Id)
                {
                    Logger.Debug("Connection bind was deleted");
                    connection.IsDeleted = true;
                    await connection.RefreshAsync();
                }
            }
        }
    }
}
