using System;
using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games
{
    /// <summary>
    /// Represents a death in Werewolf.
    /// </summary>
    public class WerewolfDeath
    {
        // The ID of the person that died
        public ulong UserId { get; set; }

        // This list of killers, if any
        public List<ulong> Killers { get; set; }

        // The method at which they were killed
        public WerewolfDeathMethod Method { get; set; }

        // The time at which they were killed
        public DateTime DiedAt { get; set; }

        // This keep track if a death was handled
        public bool Handled { get; set; }
    }
}