using System;

namespace Arcadia
{
    [Flags]
    public enum BoostType
    {
        Orite = 1, // Boosts Orite
        Chips = 2, // Boosts Chips
        Debt = 4, // Boosts Debt
        Tokens = 8, // Boosts Tokens
        Exp = 16, // Boosts Exp
        All = Orite | Chips | Debt | Tokens | Exp // Boosts any of the types
    }

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
    }
}
