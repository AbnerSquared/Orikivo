using System;
using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic player.
    /// </summary>
    public interface IPlayer
    {
        // Make the multiplayer system platform-independant
        IUser User { get; }

        /// <summary>
        /// Represents the time at which this player joined.
        /// </summary>
        DateTime JoinedAt { get; }

        bool IsPlaying { get; }
    }
}
