using System;
using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public interface IPlayer
    {
        ulong Id { get; }

        string Name { get; }

        DateTime JoinedAt { get; }

        bool IsPlaying { get; }
    }
}
