using System.Collections.Generic;
using System.Linq;
using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Contains a collection of stat identifiers.
    /// </summary>
    internal static class Stats
    {
        private static readonly Dictionary<string, string[]> Replacements = new Dictionary<string, string[]>
        {
            ["generic:items_crafted"] = new []{ "generic:times_crafted" }
        };

        internal static void RenameIds(ArcadeUser user)
        {
            foreach((string name, string[] outdated) in Replacements)
            {
                List<string> possible = user.Stats.Keys
                    .Select(x => x.ContainsAny(out string match, outdated) ? match : null)
                    .Where(x => Check.NotNull(x))
                    .ToList();

                if (possible.Count != 1)
                    continue;

                Var.Rename(user, possible.First(), name);
                return;
            }
        }

        internal static readonly string QuestCapacity = "var:quest_capacity";
        internal static readonly string TotalCompletedQuests = "quest:total_completed";
        internal static readonly string TotalAssignedQuests = "quest:total_assigned";
        internal static readonly string LastAssignedQuest = "quest:last_assigned";
        internal static readonly string LastSkippedQuest = "quest:last_skipped";

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
        internal static readonly string ItemsBroken = "generic:items_broken";
        internal static readonly string ItemsGifted = "generic:items_gifted";
        internal static readonly string ItemsCrafted = "generic:items_crafted";
        internal static readonly string ItemsBought = "generic:items_bought";
        internal static readonly string TimesUpgraded = "generic:times_upgraded";
        internal static readonly string BoostersUsed = "generic:boosters_used";

        internal static readonly string DailyStreak = "daily:current_streak";
        internal static readonly string LongestDailyStreak = "daily:longest_streak";
        internal static readonly string TimesDaily = "daily:times_used";

        internal static readonly string VoteStreak = "vote:current_streak";
        internal static readonly string LongestVoteStreak = "vote:longest_streak";
        internal static readonly string TimesVoted = "vote:times_used";

        internal static readonly string MultiplayerDailyGameCount = "multiplayer:daily_game_count";
        internal static readonly string MultiplayerGamesPlayed = "multiplayer:games_played";
    }
}
