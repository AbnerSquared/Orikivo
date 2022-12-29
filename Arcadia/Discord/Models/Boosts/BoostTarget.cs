using System;

namespace Arcadia
{
    [Flags]
    public enum BoostTarget
    {
        /// <summary>
        /// Targets income related to Orite.
        /// </summary>
        Money = 1,
        /// <summary>
        /// Targets income related to voting.
        /// </summary>
        Vote = 2,

        /// <summary>
        /// Targets income related to debt.
        /// </summary>
        Debt = 4,

        /// <summary>
        /// Targets income related to chips.
        /// </summary>
        Chips = 8,

        /// <summary>
        /// Targets experience received.
        /// </summary>
        Exp = 16

        /*
        /// <summary>
        /// Targets income related to dailies.
        /// </summary>
        Daily = 16,

        /// <summary>
        /// Targets income related to quests.
        /// </summary>
        Quest = 32,

        /// <summary>
        /// Targets income related to challenges.
        /// </summary>
        Challenge = 64
            */
    }
}
