using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    public class Game
    {
        private BaseSocketClient _client;
        private GameEventHandler _events;
        private GameConfig _config;

        public Game(BaseSocketClient client, GameConfig config)
        {
            _client = client;
            _events = new GameEventHandler();
            Id = KeyBuilder.Generate(8);
            Logger = new GameLogger();
            Lobby = new GameLobby(config, _events);
            Monitor = new GameMonitor(); // this needs to build the generic lobby display
            // the game display can be built in game properties.
            // _events.UserJoined += ...

        }

        public string Id { get; }
        public string SessionName { get; private set; }
        public GamePrivacy Privacy { get; private set; }
        public string Password { get; private set; }
        public bool IsProtected => Checks.NotNull(Password);
        public GameMode Mode { get; private set; }
        public GameState State { get; private set; }
        public GameMonitor Monitor { get; } // the root display for all receivers.
        // create UpdateDisplay(...);
        public GameLogger Logger { get; } // logs all events in the game.
        public GameLobby Lobby { get; } // contains all of the users and receivers.
        // attach the client onto the game, delete it when done.
        internal GameClient Client { get; private set; }
        public List<User> Users => Lobby.Users;
        public List<GameReceiver> Receivers => Lobby.Receivers;

        // detect message mechanics
        private async Task OnMessageReceivedAsync(SocketMessage message) { }
        //if (userId == _client.CurrentUser.Id)

        // this is what's used to start the lobby client.
        public async Task BootAsync(OriCommandContext context)
        {
            if (State == GameState.Active)
                throw new Exception("The game has already started.");

            await Lobby.BootAsync(context);
            // create the lobby handle here...
        }

        public async Task<GameResult> StartAsync()
        {
            if (State == GameState.Active)
                throw new Exception("The game has already started.");

            if (!Lobby.IsInitialized)
                throw new Exception("The game hasn't been initialized yet.");

            if (!Lobby.CanStart)
                throw new Exception("The game does not meet the criteria to start.");

            Client = new GameClient(this, _client, _events);
            return await Client.StartAsync();
        }

        internal async Task CloseAsync()
        {
            if (State == GameState.Active)
                await Client.StopAsync("The game is being closed due to unexpected reasons.", TimeSpan.FromSeconds(3));

            _client.MessageReceived -= OnMessageReceivedAsync;
            await Lobby.ClearAsync();
        }

        public bool ContainsUser(ulong userId)
            => Lobby.ContainsUser(userId);
        public bool ContainsGuild(ulong guildId)
            => Lobby.ContainsGuild(guildId);

        public override string ToString()
            => $"**{Lobby.Name}** #{Id}\n**{(State == GameState.Active ? "In Progress" : "Open")}** • (**{Lobby.UserCount}** of **{Lobby.UserLimit}**)\n`{Lobby.Mode}`";
    }
}
