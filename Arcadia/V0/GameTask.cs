using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Unstable
{
    /// <summary>
    /// Represents a generic user that defines their back-end game state.
    /// </summary>
    public class Identity
    {
        public IUser User;
        public ulong Id;
        public string Name;
        public GameState State;
        public bool IsHost;
        public DateTime JoinedAt;
    }

    // represents a generic player for a game
    public interface IPlayer
    {
        Identity Identity { get; }
        IEnumerable<GameProperty> GetProperties();
    }

    // marks a property as a game property
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GamePropertyAttribute : Attribute { }

    // represents a generic attribute for a game
    public class GameProperty
    {
        public string Id;
        public object Value;
        public object DefaultValue;
        public Type ValueType;
    }

    public enum GameState
    {
        Waiting, // the user is in the lobby
        Watching, // the user is only watching the game
        Playing // the user is watching this game
    }
}
