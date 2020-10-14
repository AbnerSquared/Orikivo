using System;
using Orikivo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Orikivo.Framework;
using Format = Orikivo.Format;

namespace Arcadia.Multiplayer
{
    public class GameServer
    {
        private readonly GameManager _manager;
        private readonly List<Player> _players;

        private void DrawHeader()
        {
            GameDetails gameDetails = _manager.DetailsOf(GameId);

            string gameName = GameId;
            string playerCounter = $"**{Players.Count:##,0}** {Format.TryPluralize("player", Players.Count)}";

            if (gameDetails != null)
            {
                gameName = gameDetails.Name;
                playerCounter = $"**{Players.Count:##,0}**/**{gameDetails.PlayerLimit:##,0}** {Format.TryPluralize("player", gameDetails.PlayerLimit)}";
            }

            GetBroadcast(GameState.Waiting).Content
                .GetComponent(LobbyVars.Header)
                .Draw(Name, Id, Format.Title(gameName, gameDetails?.Icon), playerCounter);
        }

        private void AddConsoleText(string text, bool draw = true)
        {
            DisplayContent waiting = GetBroadcast(GameState.Waiting).Content;
            DisplayContent editing = GetBroadcast(GameState.Editing).Content;

            waiting.GetGroup(LobbyVars.Console).Append(text);
            editing.GetGroup(LobbyVars.Console).Append(text);

            if (!draw)
                return;

            waiting.GetComponent(LobbyVars.Console).Draw();
            editing.GetComponent(LobbyVars.Console).Draw(Name);
        }

        private void DrawConfig()
        {
            GameDetails details = _manager.DetailsOf(GameId);
            string gameName = Check.NotNull(details?.Name) ? details?.Name : GameId;
            GetBroadcast(GameState.Editing).Content["config"].Draw(Name, Privacy, gameName, AllowSpectators, AllowInvites, AllowChat);
        }

        private void DrawGameConfig()
        {
            DisplayContent editing = GetBroadcast(GameState.Editing).Content;

            if (!_manager.Games.ContainsKey(GameId))
                return;

            if (!Check.NotNullOrEmpty(Options))
            {
                editing["game_config"].Active = false;
                return;
            }

            editing["game_config"].Active = true;
            editing["game_config"].Draw(
                Options.Select(x => $"{x.Name}: **{x.Value}**"),
                _manager.DetailsOf(GameId).Name);
        }

        private void DrawConsole()
        {
            GetBroadcast(GameState.Editing).Content.GetComponent(LobbyVars.Console).Draw(Name);
            GetBroadcast(GameState.Waiting).Content.GetComponent(LobbyVars.Console).Draw();
        }

        private void LoadGameConfig()
        {
            if (string.IsNullOrWhiteSpace(GameId) || !_manager.Games.ContainsKey(GameId))
            {
                return;
            }

            GameBase game = _manager.GetGame(GameId);

            if (game != null)
            {
                Options = game.Options;
                return;
            }

            Options = new List<GameOption>();
            return;
        }

        public GameDetails GetGameDetails()
            => _manager.DetailsOf(GameId);

        /// <summary>
        /// Asynchronously creates a new <see cref="GameServer"/>.
        /// </summary>
        internal static async Task<GameServer> CreateAsync(GameManager manager, IUser user, IMessageChannel channel, ServerProperties properties = null)
        {
            properties ??= ServerProperties.GetDefault(user.Username);

            if (string.IsNullOrWhiteSpace(properties.GameId))
                properties.GameId = manager.DefaultGameId;

            if (manager.ReservedUsers.ContainsKey(user.Id))
                throw new ReservedException(user.Id, EntityType.User);

            if (manager.ReservedChannels.ContainsKey(channel.Id))
                throw new ReservedException(channel.Id, EntityType.Channel);

            var server = new GameServer(manager, user, properties);

            server.DrawHeader();
            server.AddConsoleText($"[Console] {user.Username} has joined.");

            manager.ReservedUsers.Add(user.Id, server.Id);
            manager.ReservedChannels.Add(channel.Id, server.Id);

            var connectionProperties = new ConnectionProperties
            {
                AutoRefreshCounter = 4,
                CanDeleteMessages = true,
                BlockInput = false,
                Frequency = 0,
                State = GameState.Waiting
            };

            var connection = await ServerConnection.CreateAsync(channel, server, connectionProperties);

            if (user is IGuildUser guildUser)
            {
                connection.Type = ConnectionType.Guild;
                connection.GuildId = guildUser.GuildId;
            }

            server.Connections.Add(connection);
            manager.Servers.Add(server.Id, server);

            return server;
        }

        private GameServer(GameManager manager, IUser host, ServerProperties properties = null)
        {
            properties ??= ServerProperties.GetDefault(host.Username);
            _manager = manager;
            HostId = host.Id;
            Id = KeyBuilder.Generate(8);
            Name = properties.Name;
            GameId = properties.GameId;
            Privacy = properties.Privacy;
            Broadcasts = DisplayBroadcast.GetReservedBroadcasts();
            Connections = new List<ServerConnection>();
            Invites = new List<ServerInvite>();
            AllowedActions = properties.AllowedActions;
            _players = new List<Player>
            {
                new Player(this, host)
            };
            Destroyed = false;
            LastUpdated = DateTime.UtcNow;
            LoadGameConfig();
        }

        internal bool Destroyed { get; set; }

        public bool IsFull => _manager.Games.ContainsKey(GameId) && Players.Count >= _manager.DetailsOf(GameId).PlayerLimit;

        /// <summary>
        /// Represents the unique identifier for this <see cref="GameServer"/>.
        /// </summary>
        public string Id { get; }

        public string Name { get; private set; }

        public string GameId { get; private set; }

        public Privacy Privacy { get; private set; }

        public ulong HostId { get; private set; }

        public ServerAllow AllowedActions { get; private set; }

        public bool AllowSpectators
        {
            get => AllowedActions.HasFlag(ServerAllow.Spectate);
            set
            {
                if (value)
                    AllowedActions |= ServerAllow.Spectate;
                else
                    AllowedActions &= ~ServerAllow.Spectate;
            }
        }

        public bool AllowChat
        {
            get => AllowedActions.HasFlag(ServerAllow.Chat);
            set
            {
                if (value)
                    AllowedActions |= ServerAllow.Chat;
                else
                    AllowedActions &= ~ServerAllow.Chat;
            }
        }

        public bool AllowInvites
        {
            get => AllowedActions.HasFlag(ServerAllow.Invite);
            set
            {
                if (value)
                    AllowedActions |= ServerAllow.Invite;
                else
                    AllowedActions &= ~ServerAllow.Invite;
            }
        }

        public Player Host => GetPlayer(HostId);

        // all base displays for the game server
        public List<DisplayBroadcast> Broadcasts { get; }

        // everyone connected to the lobby
        public IReadOnlyList<Player> Players => _players;

        // all of the channels that this lobby is connected to
        public List<ServerConnection> Connections { get; }

        public List<ServerInvite> Invites { get; }

        public List<GameOption> Options { get; private set; }

        public GameSession Session { get; internal set; }

        public DateTime LastUpdated { get; private set; }

        public async Task<bool> StartGameAsync()
        {
            if (Session != null)
            {
                AddConsoleText("[Console] A game is already in progress.");
                return true;
            }

            if (!_manager.Games.ContainsKey(GameId))
            {
                AddConsoleText("[Console] Unable to find the specified game.");
                return false;
            }

            GameDetails details = _manager.DetailsOf(GameId);

            if (details == null)
            {
                AddConsoleText($"[Console] Unable to initialize a session of '{GameId}'");
                return false;
            }
            else
            {
                string name = details.Name ?? "UNKNOWN_GAME";

                if (Players.Count >= details.RequiredPlayers &&
                    Players.Count <= details.PlayerLimit)
                {
                    try
                    {
                        await _manager.GetGame(GameId).BuildAsync(this);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        AddConsoleText($"[Console] An exception has been thrown while initializing {name}.");
                        DestroyCurrentSession();
                        return false;
                    }
                }

                if (Players.Count >= details.PlayerLimit)
                {
                    AddConsoleText($"[Console] There are too many players in this server to start a game of {name}.");
                    return false;
                }

                if (Players.Count < details.RequiredPlayers)
                {
                    int requiredPlayers = details.RequiredPlayers - Players.Count;
                    string conjoin = requiredPlayers > 1 ? "are" : "is";
                    string buffer = $"[Console] {requiredPlayers} more {Format.TryPluralize("player", requiredPlayers)} {conjoin} required to start a game of {name}.";

                    AddConsoleText(buffer);
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> UpdateNameAsync(string name)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (!Check.NotNull(name))
            {
                AddConsoleText("[Console] A name cannot be empty or consist of only whitespace characters.");
                return false;
            }

            if (Format.IsSensitive(name))
            {
                AddConsoleText("[Console] A name cannot contain any Markdown sequence characters.");
                return false;
            }

            if (name.Length > ServerProperties.MaxNameLength)
            {
                AddConsoleText($"[Console] A name must be less than or equal to {ServerProperties.MaxNameLength} characters in size.");
                return false;
            }

            Name = name;
            AddConsoleText($"[Console] The name of this server has been renamed to \"{Name}\".");
            DrawConfig();
            DrawConsole();
            DrawHeader();

            await UpdateAsync();
            return true;
        }

        public async Task<bool> SetOptionAsync(string id, string value)
        {
            if (!_manager.Games.ContainsKey(GameId))
            {
                AddConsoleText("[Console] Unable to load the options for the specified game.");
                return false;
            }

            foreach (GameOption option in Options)
            {
                if (id != option.Id)
                    continue;

                if (TypeParser.TryParse(option.ValueType, value, out object result))
                {
                    option.Value = result;
                    AddConsoleText($"[Console] Set \"{option.Name}\" to the specified value.");
                    DrawGameConfig();
                    await UpdateAsync();
                    return true;
                }
            }

            AddConsoleText("[Console] Unable to find the specified option.");
            return false;
        }

        public async Task<bool> UpdatePrivacyAsync(Privacy privacy)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (privacy == Privacy)
                return true;

            Privacy = privacy;
            AddConsoleText($"[Console] The privacy of this server has been set to {Privacy}.");
            DrawConfig();

            await UpdateAsync();
            return true;
        }

        public async Task<bool> ToggleSpectateAsync()
        {
            if (Session != null)
            {
                AddConsoleText($"[Console] Spectator toggle cannot be set while a game session is active.");
                return false;
            }

            AllowSpectators = !AllowSpectators;
            AddConsoleText($"[Console] Server spectating is now {(AllowSpectators ? "enabled" : "disabled")}.");

            await UpdateAsync();
            return true;
        }

        public async Task ToggleChatAsync()
        {
            AllowChat = !AllowChat;
            AddConsoleText($"[Console] Server chat is now {(AllowChat ? "enabled" : "disabled")}.");

            await UpdateAsync();
        }

        public async Task ToggleInviteAsync()
        {
            AllowInvites = !AllowInvites;
            AddConsoleText($"[Console] Server invites are now {(AllowInvites ? "enabled" : "disabled")}.");

            await UpdateAsync();
        }

        public async Task<bool> UpdateGameAsync(string gameId)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (string.IsNullOrWhiteSpace(gameId) || !_manager.Games.ContainsKey(gameId))
            {
                AddConsoleText("[Console] An unknown game mode was specified.");
                return false;
            }

            GameId = gameId;

            AddConsoleText($"[Console] The game mode has been set to '{GameId}'.");

            LoadGameConfig();
            DrawConfig();
            DrawGameConfig();
            DrawHeader();

            await UpdateAsync();
            return true;
        }

        public async Task<bool> StartAsync()
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            await UpdateAsync();
            return true;
        }

        public async Task<Player> GetOrAddPlayerAsync(IUser user)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (GetPlayer(user.Id) != null)
                return GetPlayer(user.Id);

            if (!await AddPlayerAsync(user))
                throw new Exception("Unable to create a new player for the specified server");

            return GetPlayer(user.Id);
        }

        public async Task<bool> AddConnectionAsync(Player player, ConnectionProperties properties = null)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (player == null)
                return false;

            if (player.Server.Id != Id)
                return false;

            PlayerChannel channel = await player.GetOrCreateChannelAsync();

            if (GetConnection(channel.Id) != null)
                return true;

            if (_manager.ReservedChannels.ContainsKey(channel.Id))
                return false;

            var connection = await ServerConnection.CreateAsync(player, properties);

            Connections.Add(connection);
            _manager.ReservedChannels.Add(channel.Id, Id);

            if (connection.DeleteMessages)
                connection.DeleteMessages = false;

            return true;
        }

        public async Task<bool> AddConnectionAsync(IMessageChannel channel, ConnectionProperties properties = null)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (GetConnection(channel.Id) != null)
                return true;

            if (_manager.ReservedChannels.ContainsKey(channel.Id))
                return false;

            var connection = await ServerConnection.CreateAsync(channel, this, properties);

            Connections.Add(connection);
            _manager.ReservedChannels.Add(channel.Id, Id);

            if (connection.DeleteMessages)
                await _manager.SetDeleteStateAsync(connection);

            return true;
        }

        public async Task<bool> RemoveConnectionAsync(ulong channelId)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            ServerConnection connection = GetConnection(channelId);

            if (connection == null)
                return false;

            //if (Session != null)
            //    DestroyCurrentSession();

            Connections.Remove(connection);
            _manager.ReservedChannels.Remove(channelId);

            var removedUserIds = new List<ulong>();

            foreach ((Player player, List<ulong> channelIds) in await GetPlayerConnectionsAsync())
            {
                if (channelIds.Count == 1 && channelIds.Contains(channelId))
                    removedUserIds.Add(player.User.Id);
            }

            await RemovePlayerGroupAsync(removedUserIds, "All visible connections were destroyed");
            await connection.DestroyAsync();
            connection.Destroyed = true;
            return true;
        }

        public async Task<bool> AddPlayerAsync(IUser user)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            if (IsFull)
                return false;

            if (GetPlayer(user.Id) != null)
                return true;

            if (_manager.ReservedUsers.ContainsKey(user.Id))
                return false;

            _players.Add(new Player(this, user));
            _manager.ReservedUsers.Add(user.Id, Id);

            DrawHeader();
            AddConsoleText($"[Console] {user.Username} has joined.");
            await UpdateAsync();
            return true;
        }

        // This removes all players at once without forcing an update on each user
        public async Task RemovePlayerGroupAsync(List<ulong> userIds, string reason = null)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            foreach (ulong userId in userIds)
            {
                Player player = GetPlayer(userId);

                if (player == null)
                    continue;

                _players.Remove(player);
                _manager.ReservedUsers.Remove(userId);

                if (_players.Count == 0)
                {
                    await _manager.DestroyServerAsync(this);
                    return;
                }

                DrawHeader();
                AddConsoleText($"[Console] {player.User.Username} has left.", false);

                if (Check.NotNull(reason))
                {
                    PlayerChannel channel = await player.GetOrCreateChannelAsync();
                    await channel.SendAsync($"You were removed from the server.\nReason: {reason}");
                }
            }

            if (userIds.Contains(HostId))
            {
                Player oldest = _players.OrderBy(x => x.JoinedAt).First();

                HostId = oldest.User.Id;

                AddConsoleText($"[Console] {oldest.User.Username} is now the host.");
                ChangeState(GameState.Editing, GameState.Waiting);
            }

            await UpdateAsync();
        }

        public async Task<bool> RemovePlayerAsync(ulong userId, string reason = null)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            Player player = GetPlayer(userId);

            if (player == null)
                return false;

            _players.Remove(player);
            _manager.ReservedUsers.Remove(userId);

            if (_players.Count == 0)
            {
                await _manager.DestroyServerAsync(this);
                return true;
            }

            DrawHeader();
            AddConsoleText($"[Console] {player.User.Username} has left.");

            if (userId == HostId)
            {
                Player oldest = _players.OrderBy(x => x.JoinedAt).First();

                HostId = oldest.User.Id;

                AddConsoleText($"[Console] {oldest.User.Username} is now the host.");
                ChangeState(GameState.Editing, GameState.Waiting);
            }

            if (Check.NotNull(reason))
            {
                PlayerChannel channel = await player.GetOrCreateChannelAsync();
                await channel.SendAsync($"You were removed from the server.\nReason: {reason}");
            }

            await UpdateAsync();
            return true;
        }

        public ServerConnection GetConnection(ulong channelId)
            => Connections.FirstOrDefault(x => x.ChannelId == channelId);

        public bool HasConnection(ulong channelId)
            => Connections.Any(x => x.ChannelId == channelId);

        public void GroupAll(string group)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            foreach (ServerConnection connection in Connections)
                connection.Group = group;
        }

        public DisplayBroadcast GetSpectatorBroadcast()
        {
            if (Session == null)
                return null;

            if (!Session.CanSpectate || Session.SpectateFrequency == 0)
                return null;

            return GetBroadcast(Session.SpectateFrequency);
        }

        public IEnumerable<ServerConnection> GetGroup(string group)
            => Connections.Where(x => x.Group == group);

        public void Ungroup(string group)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            foreach (ServerConnection connection in Connections)
            {
                if (connection.Group == group)
                    connection.Group = null;
            }
        }

        public DisplayBroadcast GetBroadcast(int frequency)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            return Broadcasts.FirstOrDefault(x => x.Frequency == frequency);
        }

        public DisplayBroadcast GetBroadcast(GameState state)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            return Broadcasts.FirstOrDefault(x => x.State == state);
        }

        public void SetStateFrequency(GameState state, int frequency)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            foreach (ServerConnection connection in Connections)
            {
                if (state.HasFlag(connection.State))
                    connection.Frequency = frequency;
            }
        }

        public IEnumerable<ServerConnection> GetConnectionsInState(GameState state)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            return Connections.Where(x => state.HasFlag(x.State));
        }

        // Updates all connections with the specified state to the new state
        public void ChangeState(GameState previous, GameState current)
        {
            if (Destroyed)

                throw new Exception("This server has been destroyed");
            foreach (ServerConnection connection in Connections.Where(x => x.State.HasFlag(previous)))
                connection.State = current;
        }

        public Player GetPlayer(ulong userId)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            return Players.FirstOrDefault(x => x.User.Id == userId);
        }

        public async Task<Dictionary<Player, List<ulong>>> GetPlayerConnectionsAsync()
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            var playerConnections = new Dictionary<Player, List<ulong>>();

            foreach(Player player in Players)
            {
                var channelIds = new List<ulong>();

                foreach (ServerConnection connection in Connections)
                {
                    if (connection.Channel == null)
                        continue;

                    if (await connection.Channel.GetUserAsync(player.User.Id) == null)
                        continue;

                    channelIds.Add(connection.ChannelId);
                }

                playerConnections[player] = channelIds;
            }

            return playerConnections;
        }

        internal void EndCurrentSession()
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            _manager.EndSession(Session);
            DestroyCurrentSession();
        }

        // this ends the current session a server has active
        public void DestroyCurrentSession(string reason = null)
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            foreach (ServerConnection connection in Connections)
            {
                if (!(GameState.Watching | GameState.Playing).HasFlag(connection.State))
                    continue;

                connection.Frequency = 0;
                connection.State = GameState.Waiting;
                connection.Group = null;
                connection.BlockInput = false;
            }

            List<ServerConnection> toRemove  = Connections
                .Where(x => x.Origin == OriginType.Session || x.Origin == OriginType.Unknown && x.CreatedAt > Session.StartedAt).ToList();

            foreach (ServerConnection connection in toRemove)
                RemoveConnectionAsync(connection.ChannelId).ConfigureAwait(false).GetAwaiter().GetResult();

            // If this display is not a reserved channel, remove it
            Broadcasts.RemoveAll(x => !x.Reserved);
            Session.DisposeQueue();
            Session = null;

            AddConsoleText($"[Console] {reason ?? "The current session has ended."}");
        }

        public async Task UpdateAsync()
        {
            if (Destroyed)
                throw new Exception("This server has been destroyed");

            foreach (ServerConnection connection in Connections)
            {
                Logger.Debug($"Refreshing {connection.ChannelId} - {connection.State.ToString()}");

                try
                {
                    await connection.RefreshAsync();
                }
                catch (Exception e)
                {
                    Logger.Debug($"Error thrown when refreshing connection\n{e.Message}\n{e.StackTrace}");
                }
            }

            LastUpdated = DateTime.UtcNow;
        }
    }
}
