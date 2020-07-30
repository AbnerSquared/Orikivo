using System;

namespace Arcadia.Multiplayer.Games
{
    public class WerewolfKill
    {
        public ulong UserId { get; set; }
        public WerewolfKillMethod Method { get; set; }

        // the time at which they were killed
        public DateTime DiedAt { get; set; }
    }
}