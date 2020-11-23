using System;

namespace Arcadia.Multiplayer.Games
{
    /// <summary>
    /// Represents the status of a player in Werewolf.
    /// </summary>
    [Flags]
    public enum WolfStatus
    {
        // The player is currently dead
        Dead = 1,

        // The player is marked for death
        Marked = 2,

        // The player is currently hurt
        Hurt = 4,

        // The player is currently protected
        Protected = 8,

        // The player is currently convicted
        Convicted = 16,

        // The player is currently accusing someone
        Accusant = 32,

        // The player's role has been globally revealed
        Revealed = 64
    }
}
