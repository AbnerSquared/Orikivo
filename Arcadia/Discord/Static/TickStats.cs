namespace Arcadia
{
    /// <summary>
    /// Contains a collection of Tick stat identifiers.
    /// </summary>
    internal static class TickStats
    {
        internal static readonly string TimesPlayed = "doubler:times_played";
        internal static readonly string TimesWon = "doubler:times_won";
        internal static readonly string TimesWonExact = "doubler:times_won_exact";
        internal static readonly string TimesLost = "doubler:times_lost";

        internal static readonly string TotalBet = "doubler:total_bet";
        internal static readonly string TotalWon = "doubler:total_won";

        internal static readonly string LongestWin = "doubler:longest_win";
        internal static readonly string LongestWinExact = "doubler:longest_win_exact";
        internal static readonly string LargestWin = "doubler:largest_win";
        internal static readonly string LargestWinSingle = "doubler:largest_win_single";

        internal static readonly string CurrentWinStreak = "doubler:current_win_streak";
        internal static readonly string CurrentWinAmount = "doubler:current_win_amount";
        internal static readonly string CurrentLossStreak = "doubler:current_loss_streak";
    }
}