using System;
using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games.Werewolf
{
    public class WerewolfDeath
    {
        public WerewolfDeath(ulong userId, WerewolfDeathMethod method)
        {
            UserId = userId;
            DiedAt = DateTime.UtcNow;
            Method = method;
            KillerIds = new List<ulong>();
            Handled = false;
        }

        public WerewolfDeath(ulong userId, WerewolfDeathMethod method, ulong killerId)
        {
            UserId = userId;
            DiedAt = DateTime.UtcNow;
            Method = method;
            KillerIds = new List<ulong> { killerId };
            Handled = false;
        }

        public WerewolfDeath(ulong userId, WerewolfDeathMethod method, List<ulong> killerIds)
        {
            UserId = userId;
            DiedAt = DateTime.UtcNow;
            Method = method;
            KillerIds = killerIds ?? new List<ulong>();
            Handled = false;
        }

        public ulong UserId { get; }

        public DateTime DiedAt { get; }

        public WerewolfDeathMethod Method { get; }

        public List<ulong> KillerIds { get; }

        public bool Handled { get; internal set; }
    }
}
