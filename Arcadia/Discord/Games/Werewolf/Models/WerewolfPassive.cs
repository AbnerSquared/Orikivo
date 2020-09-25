using System;

namespace Arcadia.Multiplayer.Games.Werewolf
{
    [Flags]
    public enum WerewolfPassive
    {
        None = 0,

        /// <summary>
        /// Players are able to take a hit, dying once night falls.
        /// </summary>
        Tough = 1,

        /// <summary>
        /// Forces the player to always vote for survival.
        /// </summary>
        Pacifist = 2,

        // This always forces the player to vote for death
        /// <summary>
        /// Forces the player to always vote for death.
        /// </summary>
        Militarist = 4,

        /// <summary>
        /// Players are dead at the start, but are spectators and can write a single letter at the start of each day.
        /// </summary>
        Ghost = 8,

        /// <summary>
        /// Players appear as a wolf to the seer.
        /// </summary>
        Wolfish = 16,

        // NOTE: If the Seer dies, everyone OR the first one with this passive becomes a Seer
        /// <summary>
        /// Players inherit the role of a seer if the current seer is killed.
        /// </summary>
        Apprentice = 32
    }
}
