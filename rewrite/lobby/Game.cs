using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    public enum ReceiverChannel
    {
        Lobby = 1,
        Game = 2, 
        Spectator = 3
    }
    public class Game
    {
        private BaseSocketClient _client;
        private readonly NodeMetadata DefaultNodeGroup;
        private GameEventHandler _eventHandler;
        internal GameLogger _logger;
        public Game(BaseSocketClient client, LobbyConfig lobbyConfig, DisplayConfig displayConfig = null)
        {
            Id = KeyBuilder.Generate(8);
            // set client.MessageReceived on creation, and remove it on close.
            _client = client; // exposes methods required to check users.
            _logger = new GameLogger();
            _eventHandler = new GameEventHandler();
            Lobby = new GameLobby(lobbyConfig, _eventHandler);
            Display = new Display(displayConfig);
            DefaultNodeGroup = Display.AddGroup(new StringNodeGroup(new NodeGroupProperties()));
            _client.MessageReceived += OnMessageReceivedAsync;
            _eventHandler.DisplayUpdated += OnDisplayUpdatedAsync;
            _eventHandler.ReceiverConnected += OnReceiverConnectedAsync;
            _eventHandler.UserJoined += OnUserJoinedAsync;
            _eventHandler.GameStarted += OnGameStartedAsync;
        }

        public string Id { get; }

        public GameState State { get; private set; } // describes what the game state currently is; if the main game is idle, active, or etc.
        public GameLobby Lobby { get; private set; }
        // this handles connected receivers and users.
        public Display Display { get; private set; }
        // this handles what the receivers see.
        internal Queue<Task> DisplayQueue { get; private set; } = new Queue<Task>();
        // this handles updating the display over a ratelimit to prevent issues.
        public List<Receiver> Receivers => Lobby.Receivers;
        // from lobby.receivers
        public List<User> Users => Lobby.Users;
        // from lobby.users

        // private List<Trigger> _triggers;

        // handle commands here
        internal async Task OnGameStartedAsync(Game game)
        {
            foreach (Receiver receiver in Lobby.Receivers)
                receiver.State = GameState.Active;
        }

        internal async Task OnGameEndedAsync(Game game)
        {
            foreach (Receiver receiver in Lobby.Receivers)
                receiver.State = GameState.Inactive;
        }

        internal async Task OnReceiverConnectedAsync(Receiver receiver, GameLobby lobby)
        {
            Console.WriteLine($"-- #{Id}.ReceiverConnected = {receiver.Id} --");
            _logger.Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.ReceiverConnected = {receiver.Id}");
            await receiver.UpdateAsync(_client, Display);
        }
        internal async Task OnDisplayUpdatedAsync(Display previous, Display current, Game game)
        {
            Console.WriteLine($"-- #{Id}.DisplayUpdated --");
            _logger.Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.DisplayUpdated");
            Receivers.ForEach(async x => await x.UpdateAsync(_client, current));
        }
        internal async Task OnMessageReceivedAsync(SocketMessage message)
        {
            ulong userId = message.Author.Id;
            ulong channelId = message.Channel.Id;

            if (userId == _client.CurrentUser.Id) // if the message was from the bot
                return;
            // make sure to handle receivers based if the receivers is looking at the game or lobby
            if (!Receivers.Any(x => x.ChannelId == channelId)) // if the message was from a receiver
                return;
            if (!Users.Any(x => x.Id == userId)) // if the author of the message is a user in this lobby
                return;

            // if the receiver can delete messages
            bool canDeleteMessage = Receivers.First(x => x.ChannelId == channelId).DeleteMessages;

            await UpdateDisplayAsync("[Valid Message]");
            // handle triggers here

        }

        private async Task UpdateDisplayAsync(string content)
        {
            Display.AddAtGroup(DefaultNodeGroup.Index, content); // returns the index of that node.
            await _eventHandler.InvokeDisplayUpdatedAsync(Display, Display, this);
        }

        private async Task OnUserJoinedAsync(User user, GameLobby lobby)
        {
            Console.WriteLine($"-- #{Id}.UserJoined = {user.Id} --");
            _logger.Log($"[{DateTime.UtcNow.ToShortTimeString()}] #{Id}.UserJoined = {user.Id}");
            await UpdateDisplayAsync($"[Console] {user.Name} has joined.");
            // update the display
        }
        // a game needs to hold info about the users currently playing
        // the display that the game is currently showing
        // the collections of subscribers to the game (dedicated channels
        // the game's handler, for the actual game itself
        // and a backline handle, which manages users leaving, joining, etc.
        // and a message handler, which manages what to do when a message is received.
        internal async Task CloseAsync()
        {
            _client.MessageReceived -= OnMessageReceivedAsync;
            Console.WriteLine("-- Closing lobbies... --");
            await Lobby.ClearAsync();
            Console.WriteLine("-- Lobbies closed. --");
            //_gameEventHandler.InvokeGameClosedAsync(this);
        }

        public bool ContainsUser(ulong userId)
            => Users.Any(x => x.Id == userId);
        // public async Task DeleteAsync(); // closes _client.MessageReceived.

        public override string ToString()
        {
            return $"**{Lobby.Name}** #{Id}\n**{(State == GameState.Active ? "In Progress" : "Open")}** • (**{Lobby.UserCount}** of **{Lobby.UserLimit}**)\n`{Lobby.Mode}`";
        }
    }
}
