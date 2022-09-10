using System;
using Arcadia.Models;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic player.
    /// </summary>
    public interface IPlayer : IModel<ulong>
    {
        /// <summary>
        /// Represents the time at which this player joined.
        /// </summary>
        DateTime JoinedAt { get; }

        bool IsPlaying { get; }
    }
}
