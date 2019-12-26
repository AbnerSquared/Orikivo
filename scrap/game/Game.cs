using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // GameMessageContainer: keeps track of all messages
    public class Game
    {
        // Dependencies
        private readonly BaseSocketClient _client;
        private readonly GameEventHandler _events;

        public Game(BaseSocketClient client, GameConfig config)
        {
            _client = client;
            _events = new GameEventHandler();
            Id = KeyBuilder.Generate(8);
            Logger = new GameLogger();
            Lobby = new GameLobby(config, _events);
            Display = new GameDisplay(Id, _events, GameWindowProperties.Game); // this needs to build the generic lobby display
            // the game display can be built in game properties.
            _events.DisplayUpdated += OnDisplayUpdatedAsync;
            _events.ReceiverConnected += OnReceiverConnectedAsync;
            _client.MessageReceived += OnOffhandMessageAsync;
            _events.UserJoined += OnUserJoinedAsync;
            _events.UserLeft += OnUserLeftAsync;
        }

        // Properties
        public string Id { get; }
        public string SessionName { get; private set; }
        public GamePrivacy Privacy { get; private set; }
        public string Password { get; private set; }
        public bool IsProtected => Checks.NotNull(Password);
        public GameMode Mode { get; private set; }
        public GameState State { get; private set; }
        public GameDisplay Display { get; } // the root display for all receivers.
        // create UpdateDisplay(...);
        public GameLogger Logger { get; } // logs all events in the game.
        public GameLobby Lobby { get; } // contains all of the users and receivers.
        // attach the client onto the game, delete it when done.
        internal GameClient Client { get; private set; }
        public List<Identity> Users => Lobby.Users;
        public List<GameReceiver> Receivers => Lobby.Receivers;

        // Private Properties
        /// <summary>
        /// A private property that defines what message handler to utilize.
        /// </summary>
        private bool UseMainHandler { get; set; } = true;

        // detect message mechanics
        private async Task OnOffhandMessageAsync(SocketMessage message)
        {
            Console.WriteLine("-- Now validating active lobby message... --");
            if (!(Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Inactive) && ContainsUser(message.Author.Id)))
                return;
            if (message.Content == "start")
            {
                await Display.UpdateWindowAsync(GameState.Inactive, new ElementUpdatePacket(new Element($"[Console] A game is already in progress."), ElementUpdateMethod.AddToGroup, groupId: "elements.console"));
            }
        }
        //if (userId == _client.CurrentUser.Id)

        private async Task OnDisplayUpdatedAsync(GameDisplay display)
        {
            Console.WriteLine($"-- A display update was called. --");
            Receivers.ForEach(async x => { await x.UpdateAsync(_client, Display); Console.WriteLine($"-- ({x.Id}.{x.State}) Display updated. --"); });
        }
        private async Task OnUserJoinedAsync(Identity user, GameLobby lobby)
            => await Display.UpdateWindowAsync(GameState.Inactive, new ElementUpdatePacket(new Element($"[Console] {user.Name} has joined."), ElementUpdateMethod.AddToGroup, groupId: "elements.console"));
        private async Task OnUserLeftAsync(Identity user, GameLobby lobby)
            => await Display.UpdateWindowAsync(GameState.Inactive, new ElementUpdatePacket(new Element($"[Console] {user.Name} has left."), ElementUpdateMethod.AddToGroup, groupId: "elements.console"));

        private async Task OnReceiverConnectedAsync(GameReceiver receiver, GameLobby lobby)
            => await receiver.UpdateAsync(_client, Display);

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

        // Begins a lobby session. This is the main session listener.
        internal async Task StartSessionAsync()
        {
            while (UseMainHandler)
            {
                _client.MessageReceived -= OnOffhandMessageAsync; // Disable the Secondary Handler.

                // SecondaryHandler handles all other commands that alter config and such. Lock configuration to host, and lock changes being made during a game.
                TaskCompletionSource<bool> session = new TaskCompletionSource<bool>(); // Handles when to start/close (The main reason this Task exists)

                async Task ReadAsync(SocketMessage message)
                {
                    Console.WriteLine("-- Now validating lobby message. --");
                    // if there are no inactive receivers that exists and the user is not valid.
                    if (!(Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Inactive) && Lobby.TryGetUser(message.Author.Id, out Identity user)))
                    {
                        Console.WriteLine("-- Invalid lobby message. --");
                        return;
                    }

                    if (message.Content == "start")
                    {
                        Display.UpdateWindow(GameState.Inactive, new ElementUpdatePacket(new Element($"[Console] {Lobby.Mode} will be starting soon."), ElementUpdateMethod.AddToGroup, groupId: "elements.console"));
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        session.SetResult(true);
                    }
                    else if (message.Content == "close")
                    {
                        session.SetResult(false);
                    }
                    else
                        Display.UpdateWindow(GameState.Inactive, new ElementUpdatePacket(new Element($"[{user.Name}]: {message.Content}"), ElementUpdateMethod.AddToGroup, groupId: "elements.console"));

                    await Display.RefreshAsync();
                }

                _client.MessageReceived += ReadAsync; // sets the actual listener and stops it upon either start or close was called
                Task<bool> result = await Task.WhenAny(session.Task).ConfigureAwait(false);
                _client.MessageReceived -= ReadAsync;

                if (result.Result)
                {
                    // If result.True... Set the offhand listener which allows the lobby to still process info.
                    _client.MessageReceived += OnOffhandMessageAsync;
                    await StartAsync();
                    _client.MessageReceived -= OnOffhandMessageAsync;
                }
                else
                {
                    await CloseAsync();
                }
            }
        }

        // Attempts to start a game and parse its result.
        public async Task<GameResult> StartAsync()
        {
            if (State == GameState.Active) // Can be set at Window.Inactive
                throw new Exception("The game has already started.");

            if (!Lobby.IsInitialized)
                throw new Exception("The game hasn't been initialized yet.");

            if (!Lobby.CanStart) // Can be sent at Window.Inactive
                throw new Exception("The game does not meet the criteria to start.");

            await SetStateAsync(GameState.Active);

            Client = new GameClient(this, _client, _events);
            GameResult result = await Client.StartAsync().ConfigureAwait(false);
            Display.UpdateWindow(GameState.Active, new ElementUpdatePacket(ElementUpdateMethod.ClearGroup, groupId: "elements.chat"));
            Display.UpdateWindow(GameState.Inactive, new ElementUpdatePacket(new Element("[Console] The game has ended."), ElementUpdateMethod.AddToGroup, groupId: "elements.console"));
            await SetStateAsync(GameState.Inactive);
            Client = null;
            return result;
        }

        // UpdateStateAsync(GameState state);
        // This is used to set the global game state (if the game is active at the moment)
        private async Task SetStateAsync(GameState state)
        {
            State = state;
            Receivers.ForEach(async x =>
            {
                var oldState = x.State;
                x.State = State;
                await x.UpdateAsync(_client, Display);
                Console.WriteLine($"-- A receiver has been updated. ({x.Id}.{x.State}), {oldState} => {x.State} --");
            }); // make state updates decisive.
            Console.WriteLine("-- All receivers are now up to date. --");
        }

        // Stops all actions within a game and shuts it down.
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

        public bool ContainsChannel(ulong channelId)
            => Lobby.ContainsChannel(channelId);

        // TODO: Separate into formatting service.
        /// <summary>
        /// Returns a formatted summary of the game and its state.
        /// </summary>
        public override string ToString()
            => $"**{Lobby.Name}** #{Id}\n**{(State == GameState.Active ? "In Progress" : "Open")}** • (**{Lobby.UserCount}** of **{Lobby.UserLimit}**)\n`{Lobby.Mode}`";
    }
}
