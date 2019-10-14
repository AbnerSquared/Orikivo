using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// A handler for all possible events that can occur within a game.
    /// </summary>
    public class GameEventHandler
    {
        private readonly AsyncEvent<Func<Game, Task>> _gameStartedEvent = new AsyncEvent<Func<Game, Task>>();
        private readonly AsyncEvent<Func<Game, Task>> _gameEndedEvent = new AsyncEvent<Func<Game, Task>>();
        private readonly AsyncEvent<Func<User, GameLobby, Task>> _userJoinedEvent = new AsyncEvent<Func<User, GameLobby, Task>>();
        private readonly AsyncEvent<Func<User, GameLobby, Task>> _userLeftEvent = new AsyncEvent<Func<User, GameLobby, Task>>();
        private readonly AsyncEvent<Func<GameReceiver, GameLobby, Task>> _receiverConnectedEvent = new AsyncEvent<Func<GameReceiver, GameLobby, Task>>();
        private readonly AsyncEvent<Func<GameReceiver, GameLobby, Task>> _receiverDisconnectedEvent = new AsyncEvent<Func<GameReceiver, GameLobby, Task>>();
        private readonly AsyncEvent<Func<GameDisplay, Task>> _displayUpdatedEvent = new AsyncEvent<Func<GameDisplay, Task>>();

        public event Func<User, GameLobby, Task> UserJoined
        {
            add => _userJoinedEvent.Add(value);
            remove => _userJoinedEvent.Remove(value);
        }

        public event Func<Game, Task> GameStarted
        {
            add { _gameStartedEvent.Add(value); }
            remove { _gameStartedEvent.Remove(value); }
        }
        
        public event Func<User, GameLobby, Task> UserLeft
        {
            add { _userLeftEvent.Add(value); }
            remove { _userLeftEvent.Remove(value); }
        }
        
        public event Func<GameReceiver, GameLobby, Task> ReceiverConnected
        {
            add { _receiverConnectedEvent.Add(value); }
            remove { _receiverConnectedEvent.Remove(value); }
        }
        
        public event Func<GameReceiver, GameLobby, Task> ReceiverDisconnected
        {
            add { _receiverDisconnectedEvent.Add(value); }
            remove { _receiverDisconnectedEvent.Remove(value); }
        }

        public event Func<GameDisplay, Task> DisplayUpdated
        {
            add { _displayUpdatedEvent.Add(value); }
            remove { _displayUpdatedEvent.Remove(value); }
        }

        internal async Task InvokeUserJoinedAsync(User user, GameLobby lobby)
            => await _userJoinedEvent.InvokeAsync(user, lobby);
        internal async Task InvokeUserLeftAsync(User user, GameLobby lobby)
            => await _userLeftEvent.InvokeAsync(user, lobby);

        internal async Task InvokeReceiverConnectedAsync(GameReceiver receiver, GameLobby lobby)
            => await _receiverConnectedEvent.InvokeAsync(receiver, lobby);
        internal async Task InvokeReceiverDisconnectedAsync(GameReceiver receiver, GameLobby lobby)
            => await _receiverDisconnectedEvent.InvokeAsync(receiver, lobby);

        internal async Task InvokeDisplayUpdatedAsync(GameDisplay display) // previous, current
            => await _displayUpdatedEvent.InvokeAsync(display);

        internal async Task InvokeGameStartedAsync(Game game)
            => await _gameStartedEvent.InvokeAsync(game);
    }
}
