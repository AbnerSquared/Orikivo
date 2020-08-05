using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games
{

    // If shared peeking is used instead, Ignore listing out peekers
    // Otherwise, ignore their own UserId
    public class WerewolfPeekData
    {
        // This is the userId that this peek info is for
        public ulong UserId { get; set; }

        // this is the list of all user IDs that this ID has been revealed to.
        // If the user is a peeker AND their ID is not in this list, this ID is shown as Unknown
        public List<ulong> RevealedTo { get; set; }

        public bool Innocent { get; set; }
    }

    public class WerewolfKill
    {
        public ulong UserId { get; set; }


    }

    public enum WerewolfPeekMode
    {
        Hidden = 1, // The default: All of the information a seer is given stays with them
        Player = 2, // When a seer identifies someone, the person they chose is publicly announced at the start of a day (ONLY THE PLAYER)
        Role = 3 // When a seer identifies someone, everyone will be told if they selected a werewolf or not
    }
}