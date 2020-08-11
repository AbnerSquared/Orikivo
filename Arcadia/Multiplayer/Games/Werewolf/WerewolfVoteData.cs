using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games.Werewolf
{
    public class WerewolfVoteData
    {
        public WerewolfVoteData(ulong userId)
        {
            UserId = userId;
        }

        public ulong UserId { get; set; }

        public List<ulong> VoterIds { get; set; } = new List<ulong>();
    }
}
