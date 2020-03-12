namespace Orikivo
{
    /// <summary>
    /// Contains a collection of <see cref="Gimi"/> stat identifiers.
    /// </summary>
    internal static class GimiStats
    {
        // CONFIG
        internal static readonly string Risk = "gimi:risk";
        internal static readonly string Range = "gimi:earn";
        internal static readonly string WinDirection = "gimi:win_dir";

        // STATS
        internal static readonly string TimesPlayed = "gimi:times_played";

        internal static readonly string TimesWon = "gimi:times_won";
        internal static readonly string TotalWon = "gimi:total_won";
        internal static readonly string CurrentWinStreak = "gimi:current_win_streak";
        internal static readonly string CurrentWinAmount = "gimi:current_win_amount";
        internal static readonly string LongestWinStreak = "gimi:longest_win_streak";
        internal static readonly string LongestWinAmount = "gimi:longest_win_amount";
        internal static readonly string LargestWinStreak = "gimi:largest_win_streak";
        internal static readonly string LargestWinAmount = "gimi:largest_win_amount";

        internal static readonly string TimesLost = "gimi:times_lost";
        internal static readonly string TotalLost = "gimi:total_lost";
        internal static readonly string CurrentLossStreak = "gimi:current_loss_streak";
        internal static readonly string CurrentLossAmount = "gimi:current_loss_amount";
        internal static readonly string LongestLossStreak = "gimi:longest_loss_streak";
        internal static readonly string LongestLossAmount = "gimi:longest_loss_amount";
        internal static readonly string LargestLossStreak = "gimi:largest_loss_streak";
        internal static readonly string LargestLossAmount = "gimi:largest_loss_amount";

        internal static readonly string TimesGold = "gimi:times_gold";
        internal static readonly string CurrentGoldStreak = "gimi:current_gold_streak";
        internal static readonly string LongestGoldStreak = "gimi:longest_gold_streak";

        internal static readonly string TimesCursed = "gimi:times_cursed";
        internal static readonly string CurrentCurseStreak = "gimi:current_curse_streak";
        internal static readonly string LongestCurseStreak = "gimi:longest_curse_streak";
    }
}
