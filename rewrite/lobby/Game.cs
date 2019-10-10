using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Monitor = new GameMonitor(_events, config.Mode); // this needs to build the generic lobby display
            // the game display can be built in game properties.
            _events.DisplayUpdated += OnDisplayUpdatedAsync;
            _events.ReceiverConnected += OnReceiverConnectedAsync;
            _client.MessageReceived += OnOffhandMessageAsync;
            _events.UserJoined += OnUserJoinedAsync;
            _events.UserLeft += OnUserLeftAsync;
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
        private async Task OnOffhandMessageAsync(SocketMessage message)
        {
            Console.WriteLine("-- Now validating active lobby message... --");
            if (!(Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Inactive) && ContainsUser(message.Author.Id)))
                return;
            if (message.Content == "start")
            {
                await Monitor.UpdateDisplayAsync(GameState.Inactive, "[Console] A game is already in progress.");
            }
        }
        //if (userId == _client.CurrentUser.Id)

        private async Task OnDisplayUpdatedAsync(GameDisplay display)
        {
            Console.WriteLine($"-- A display update was called. --");
            Receivers.Where(x => x.State == display.Type).ToList().ForEach(async x => { await x.UpdateAsync(_client, Monitor); Console.WriteLine($"-- ({x.Id}.{x.State}) Display updated. --"); });
        }
        private async Task OnUserJoinedAsync(User user, GameLobby lobby)
            => await Monitor.UpdateDisplayAsync(GameState.Inactive, $"[Console] {user.Name} has joined.");
        private async Task OnUserLeftAsync(User user, GameLobby lobby)
            => await Monitor.UpdateDisplayAsync(GameState.Inactive, $"[Console] {user.Name} has left.");

        private async Task OnReceiverConnectedAsync(GameReceiver receiver, GameLobby lobby)
            => await receiver.UpdateAsync(_client, Monitor);

        // this is what's used to start the lobby client.
        public async Task BootAsync(OriCommandContext context)
        {
            Console.WriteLine($"-- ({Id}) Now creating lobby session. --");
            if (State == GameState.Active)
                throw new Exception("The game has already started.");

            await Lobby.BootAsync(context);
            // create the lobby handle here...
        }

        // two types of lobby listeners:
        // async task version is used whenever a game session can start, as a game session is a dynamic task.
        // default event version is used during a game session, as all of the commands are just quick functions

        internal async Task StartSessionAsync()
        {
            Loop:
            _client.MessageReceived -= OnOffhandMessageAsync;
            TaskCompletionSource<bool> start = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> close = new TaskCompletionSource<bool>();

            async Task ReadAsync(SocketMessage message)
            {
                Console.WriteLine("-- Now validating lobby message. --");
                // if there are no inactive receivers that exists and the user is not valid.
                if (!(Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Inactive) && Lobby.TryGetUser(message.Author.Id, out User user)))
                {
                    Console.WriteLine("-- Invalid lobby message. --");
                    return;
                }
                
                if (message.Content == "start")
                {
                    await Monitor.UpdateDisplayAsync(GameState.Inactive, $"[Console] {Lobby.Mode} will be starting soon.");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    start.SetResult(true);
                }
                else if (message.Content == "close")
                {
                    start.SetResult(false);
                }
                else
                    await Monitor.UpdateDisplayAsync(GameState.Inactive, $"[{user.Name}]: {message.Content}");
            }

            _client.MessageReceived += ReadAsync;
            Task<bool> result = await Task.WhenAny(start.Task).ConfigureAwait(false);
            _client.MessageReceived -= ReadAsync;

            if (result.Result)
            {
                _client.MessageReceived += OnOffhandMessageAsync;
                await StartAsync();
                _client.MessageReceived -= OnOffhandMessageAsync;
                goto Loop;
            }
            else
            {
                await CloseAsync();
            }
        }

        public async Task<GameResult> StartAsync()
        {
            if (State == GameState.Active)
                throw new Exception("The game has already started.");

            if (!Lobby.IsInitialized)
                throw new Exception("The game hasn't been initialized yet.");

            if (!Lobby.CanStart)
                throw new Exception("The game does not meet the criteria to start.");

            await SetStateAsync(GameState.Active);

            Client = new GameClient(this, _client, _events);
            GameResult result = await Client.StartAsync().ConfigureAwait(false);
            Monitor[GameState.Active].Content.Clear();
            Monitor[GameState.Active].Content.AppendLine("**Game**");
            await Monitor.UpdateDisplayAsync(GameState.Inactive, "[Console] The game has ended.");
            await SetStateAsync(GameState.Inactive);
            Client = null;
            return result;
        }

        private async Task SetStateAsync(GameState state)
        {
            State = state;
            Receivers.ForEach(async x =>
            {
                var oldState = x.State;
                x.State = State;
                await x.UpdateAsync(_client, Monitor);
                Console.WriteLine($"-- A receiver has been updated. ({x.Id}.{x.State}), {oldState} => {x.State} --");
            }); // make state updates decisive.
            Console.WriteLine("-- All receivers are now up to date. --");
        }

        internal async Task CloseAsync()
        {
            if (State == GameState.Active)
                await Client.StopAsync("The game is being closed due to unexpected reasons.", TimeSpan.FromSeconds(3));
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
