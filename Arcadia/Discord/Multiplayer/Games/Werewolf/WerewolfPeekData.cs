using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games.Werewolf
{
    public class WerewolfPeekData
    {
        public WerewolfPeekData(ulong userId, bool innocent)
        {
            UserId = userId;
            Innocent = innocent;
        }

        // This is the userId that this peek info is for
        public ulong UserId { get; }

        // this is the list of all user IDs that this ID has been revealed to.
        // If the user is a peeker AND their ID is not in this list, this ID is shown as Unknown
        public List<ulong> RevealedTo { get; } = new List<ulong>();

        public bool Innocent { get; }
    }
}