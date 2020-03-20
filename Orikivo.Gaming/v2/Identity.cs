using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Orikivo.Gaming
{
    /// <summary>
    /// Represents a generic player.
    /// </summary>
    public class Identity : IEntity<ulong> // information about a possible player from an explicit network reader to a lobby.
    {
        public ulong Id { get; } // user ID
        public ulong SessionId { get; } // game ID

        public IDMChannel Channel { get; }
    }

    public enum PlayerState
    {
        Playing = 1, // actively playing game.
        Inactive = 2, // afk in game
        Waiting = 4 // in lobby
    }

    // represents a temporary session for a group of players
    // implements a lobby
    // implements a LoadGame()
    // implements an InputChannel

    // represents a temporary session for a group of players

    public class GameManager
    {
        // this is what controls all input event flags.
        private readonly BaseSocketClient _client;

        // the true list of sessions
        private List<Session> _sessions;

        // You only want the list to be readonly
        public IReadOnlyList<Session> Sessions => _sessions;
    }

    public class Session
    {
        public List<Identity> Users { get; }
    }

    // represents a input reader for the a specified Identity.
    // this binds all inputs handled to a specific user.
    public class InputReader
    {
        // the identity it's reading from.
        public Identity Identity { get; }

        public string Value { get; protected set; }
        public string PreviousValue { get; protected set; }
    }

    // manages all InputController/Readers
    public class InputManager
    {

    }

    // controls what happens on each executed input.
    public class InputController
    {
        private readonly BaseSocketClient _base;

        public List<InputReader> Readers { get; }
    }

    // represents an identifiable input.
    public class Input<TKey>
    {
        public TKey Key { get; set; }
    }
}
