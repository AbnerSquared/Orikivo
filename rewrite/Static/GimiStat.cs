namespace Orikivo
{
    // TODO: Create AttributeReader, which can easily account for all of these.
    internal static class GimiStat
    {
        // Stats affect merit criteria, Attributes don't.
        internal static string Risk = "gimi:risk";
        internal static string Earn = "gimi:earn";
        internal static string GoldSlot = "gimi:gold_slot";
        internal static string CurseSlot = "gimi:curse_slot";
        internal static string WinDirection = "gimi:win_direction";
        internal static string CurrentWinStreak = "gimi:current_win_streak";
        internal static string CurrentGoldStreak = "gimi:current_gold_streak";
        internal static string CurrentLossStreak = "gimi:current_loss_streak";
        internal static string CurrentCurseStreak = "gimi:current_curse_streak";
        internal static string TimesWon = "gimi:times_won";
        internal static string TimesLost = "gimi:times_lost";
        internal static string TimesPlayed = "gimi:times_played";
        internal static string TimesWonGold = "gimi:times_won_gold";
        internal static string TimesLostCursed = "gimi:times_lost_cursed";
        internal static string LargestGoldStreakLength = "gimi:largest_gold_streak_length";
        internal static string LargestCurseStreakLength = "gimi:largest_curse_streak_length";
        internal static string LargestWinStreakLength = "gimi:largest_win_streak_length";
        internal static string LargestWinStreakAmount = "gimi:largest_win_streak_amount";
        internal static string LargestLossStreakLength = "gimi:largest_loss_streak_length";
        internal static string LargestLossStreakAmount = "gimi:largest_loss_streak_amount";
        internal static string LargestWinFromStreakLength = "gimi:largest_win_from_streak_length";
        internal static string LargestWinFromStreakAmount = "gimi:largest_win_from_streak_amount";
        internal static string LargestLossFromStreakLength = "gimi:largest_loss_from_streak_length";
        internal static string LargestLossFromStreakAmount = "gimi:largest_loss_from_streak_amount";
        internal static string TotalAmountWon = "gimi:total_amount_won";
        internal static string TotalAmountLost = "gimi:total_amount_lost";
    }
}
