using System;

namespace Arcadia.Multiplayer.Games
{
    /// <summary>
    /// Represents a death in Werewolf.
    /// </summary>
    public class WerewolfDeath
    {
        // The ID of the person that died
        public ulong UserId { get; set; }

        // The ID of the killer, if any
        public ulong? KillerId { get; set; }

        // The method at which they were killed
        public WerewolfDeathMethod Method { get; set; }

        // The time at which they were killed
        public DateTime DiedAt { get; set; }
    }
}