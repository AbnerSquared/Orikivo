﻿using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // each player could have a notepad, in which they can write small tabs as such
    // this can be a good dynamic, and be used to disrupt how notes function
    // but notes can only function during moments from which you're awake

    // in short, the game manager is one large task.
    // the manager keeps the task running, continuously cycling through the internal
    // tasks until GameTasks.IsCompleted returns true.
    // once that happens, the manager closes the main task,
    // creates update packets for each user affected (i.e. stats)

    public class GameEventHandler
    {
        // place all event flags here
        // pass this class down into each lower class
        // this way, all classes in Game have access to the events
        public event Func<User, GameLobby, Task> UserJoined
        {
            add { _userJoinedEvent.Add(value); }
            remove { _userJoinedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<User, GameLobby, Task>> _userJoinedEvent = new AsyncEvent<Func<User, GameLobby, Task>>();

        public event Func<Game, Task> GameStarted
        {
            add { _gameStartedEvent.Add(value); }
            remove { _gameStartedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Game, Task>> _gameStartedEvent = new AsyncEvent<Func<Game, Task>>();

        public event Func<User, GameLobby, Task> UserLeft
        {
            add { _userLeftEvent.Add(value); }
            remove { _userLeftEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<User, GameLobby, Task>> _userLeftEvent = new AsyncEvent<Func<User, GameLobby, Task>>();

        public event Func<GameReceiver, GameLobby, Task> ReceiverConnected
        {
            add { _receiverConnectedEvent.Add(value); }
            remove { _receiverConnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<GameReceiver, GameLobby, Task>> _receiverConnectedEvent = new AsyncEvent<Func<GameReceiver, GameLobby, Task>>();

        public event Func<GameReceiver, GameLobby, Task> ReceiverDisconnected
        {
            add { _receiverDisconnectedEvent.Add(value); }
            remove { _receiverDisconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<GameReceiver, GameLobby, Task>> _receiverDisconnectedEvent = new AsyncEvent<Func<GameReceiver, GameLobby, Task>>();

        public event Func<GameDisplay, Task> DisplayUpdated
        {
            add { _displayUpdatedEvent.Add(value); }
            remove { _displayUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<GameDisplay, Task>> _displayUpdatedEvent = new AsyncEvent<Func<GameDisplay, Task>>();

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
