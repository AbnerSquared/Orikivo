using System;
using System.Collections.Generic;
using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic receiver to a broadcast.
    /// </summary>
    public interface IReceiver
    {
        IUserMessage Message { get; }
        IMessageChannel Channel { get; }
    }

    /// <summary>
    /// Represents a generic player.
    /// </summary>
    public interface IPlayer
    {
        IUser User { get; }

        DateTime JoinedAt { get; }

        bool IsPlaying { get; }
    }
}
