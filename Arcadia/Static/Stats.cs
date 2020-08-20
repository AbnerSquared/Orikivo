using System.Collections.Generic;
using System.Reflection;

namespace Arcadia
{
    /// <summary>
    /// Contains a collection of stat identifiers.
    /// </summary>
    internal static class Stats
    {
        internal static readonly string QuestCapacity = "var:quest_capacity";
        internal static readonly string TotalCompletedQuests = "quest:total_completed";
        internal static readonly string TotalAssignedQuests = "quest:total_assigned";
        internal static readonly string LastAssignedQuest = "quest:last_assigned";
        internal static readonly string LastSkippedQuest = "quest:last_skipped";
        internal static readonly string QuestAssignTotal = "quest:assign_total";

        internal static readonly string TotalMoney = "generic:total_money";
        internal static readonly string TotalChips = "generic:total_chips";
        internal static readonly string TotalTokens = "generic:total_tokens";
        internal static readonly string TotalDebt = "generic:total_debt";

        internal static readonly string MostMoney = "generic:most_money";
        internal static readonly string MostChips = "generic:most_chips";
        internal static readonly string MostTokens = "generic:most_tokens";
        internal static readonly string MostDebt = "generic:most_debt";

        internal static readonly string TimesTraded = "generic:times_traded";
        internal static readonly string ItemsSold = "generic:items_sold";
        internal static readonly string ItemsUsed = "generic:items_used";
        internal static readonly string ItemsGifted = "generic:items_gifted";
        internal static readonly string TimesUpgraded = "generic:times_upgraded";
        internal static readonly string BoostersUsed = "generic:boosters_used";
        

        internal static readonly string DailyStreak = "daily:current_streak";
        internal static readonly string LongestDailyStreak = "daily:longest_streak";
        internal static readonly string TimesDaily = "daily:times_used";

        internal static readonly string VoteStreak = "vote:current_streak";
        internal static readonly string LongestVoteStreak = "vote:longest_streak";
        internal static readonly string TimesVoted = "vote:times_used";
    }
}
