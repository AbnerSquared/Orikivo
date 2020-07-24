namespace Arcadia
{
    /// <summary>
    /// Contains a collection of Gimi stat identifiers.
    /// </summary>
    internal static class GimiStats
    {
        // CONFIG
        internal static readonly string Risk = "gimi:risk";
        internal static readonly string Range = "gimi:earn";
        internal static readonly string WinDirection = "gimi:win_dir";

        // UPGRADES
        internal static readonly string RiskOverload = "gimi:risk_overload";
        internal static readonly string MaxExpander = "gimi:max_expander";

        // STATS
        internal static readonly string TimesPlayed = "gimi:times_played";

        internal static readonly string TimesWon = "gimi:times_won";
        internal static readonly string TimesLost = "gimi:times_lost";
        internal static readonly string TimesGold = "gimi:times_gold";
        internal static readonly string TimesCursed = "gimi:times_cursed";

        internal static readonly string TotalWon = "gimi:total_won";
        internal static readonly string TotalLost = "gimi:total_lost";

        internal static readonly string CurrentType = "gimi:current_type";
        internal static readonly string CurrentWinStreak = "gimi:current_win_streak";
        internal static readonly string CurrentWinAmount = "gimi:current_win_amount";
        internal static readonly string CurrentLossStreak = "gimi:current_loss_streak";
        internal static readonly string CurrentLossAmount = "gimi:current_loss_amount";
        internal static readonly string CurrentGoldStreak = "gimi:current_gold_streak";
        internal static readonly string CurrentCurseStreak = "gimi:current_curse_streak";

        internal static readonly string LongestWin = "gimi:longest_win";
        internal static readonly string LongestLoss = "gimi:longest_loss";

        internal static readonly string LargestWin = "gimi:largest_win";
        internal static readonly string LargestLoss = "gimi:largest_loss";
        internal static readonly string LongestGold = "gimi:longest_gold";
        internal static readonly string LongestCurse = "gimi:longest_curse";
    }
}
