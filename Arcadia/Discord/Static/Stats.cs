using System.Collections.Generic;
using System.Linq;
using Orikivo;
using Orikivo.Text;

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

        internal static class Challenge
        {
            internal static readonly string TotalCompleted = "challenge:total_completed";
            internal static readonly string MostSetsCompleted = "challenge:most_sets_completed";
            internal static readonly string SetsCompletedDaily = "challenge:sets_completed_daily";
        }

        internal static class Common
        {
            internal static readonly string QuestCapacity = "var:quest_capacity"; // Rename to var:quest_limit
            internal static readonly string TotalCompletedSpecials = "quest:total_completed_specials";
            internal static readonly string TotalCompletedQuests = "quest:total_completed";
            internal static readonly string TotalAssignedQuests = "quest:total_assigned";
            internal static readonly string LastAssignedQuest = "quest:last_assigned";
            internal static readonly string LastSkippedQuest = "quest:last_skipped";
            internal static readonly string QuestCompletedDaily = "quest:completed_daily";

            internal static readonly string TotalMoney = "generic:total_money";
            internal static readonly string TotalChips = "generic:total_chips";
            internal static readonly string TotalTokens = "generic:total_tokens";
            internal static readonly string TotalDebt = "generic:total_debt";

            internal static readonly string MostMoney = "generic:most_money";
            internal static readonly string MostChips = "generic:most_chips";
            internal static readonly string MostChipsRound = "generic:most_chips_round";
            internal static readonly string MostTokens = "generic:most_tokens";
            internal static readonly string MostDebt = "generic:most_debt";

            internal static readonly string TimesTraded = "generic:times_traded";
            internal static readonly string ItemsSold = "generic:items_sold";
            internal static readonly string ItemsBroken = "generic:items_broken";
            internal static readonly string ItemsGifted = "generic:items_gifted";
            internal static readonly string ItemsCrafted = "generic:items_crafted";
            internal static readonly string ItemsBought = "generic:items_bought";
            internal static readonly string TotalSpentShops = "generic:total_spent_shops";
            internal static readonly string ItemsOrdered = "generic:items_ordered";
            internal static readonly string TimesUpgraded = "generic:times_upgraded";
            internal static readonly string BoostersUsed = "generic:boosters_used";

            internal static readonly string DailyStreak = "daily:current_streak";
            internal static readonly string LongestDailyStreak = "daily:longest_streak";
            internal static readonly string TimesDaily = "daily:times_used";

            internal static readonly string VoteStreak = "vote:current_streak";
            internal static readonly string LongestVoteStreak = "vote:longest_streak";
            internal static readonly string TimesVoted = "vote:times_used";
        }

        internal static class Multiplayer
        {
            internal static readonly string GamesPaidDaily = "multiplayer:games_paid_daily";
            internal static readonly string GamesPlayedDaily = "multiplayer:games_played_daily";
            internal static readonly string GamesPlayed = "multiplayer:games_played";
            internal static readonly string LastGamePlayed = "multiplayer:last_game_played";
        }

        internal static class Trivia
        {
            internal static readonly string TimesPlayed = "trivia:times_played";
            internal static readonly string TimesWon = "trivia:times_won";
            internal static readonly string CurrentWinStreak = "trivia:current_win_streak";
            internal static readonly string HighestScore = "trivia:highest_score";
            internal static readonly string LongestWin = "trivia:longest_win_streak";
        }

        internal static class Werewolf
        {
            internal static readonly string TimesPlayed = "werewolf:times_played";
            internal static readonly string TimesWon = "werewolf:times_won";
        }

        internal static class Roulette
        {
            internal static readonly string TimesPlayed = "roulette:times_played";
            internal static readonly string TimesWon = "roulette:times_won";
            internal static readonly string TimesWonGreen = "roulette:times_won_green";
            internal static readonly string TimesLost = "roulette:times_lost";
            internal static readonly string TotalWon = "roulette:total_won";
            internal static readonly string TotalLost = "roulette:total_lost";
            internal static readonly string MostWon = "roulette:most_won";
        }

        internal static class Gimi
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

        internal static class Doubler
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

        internal static class BlackJack
        {
            internal static readonly string TimesPlayed = "blackjack:times_played";
            internal static readonly string TimesWon = "blackjack:times_won";
            internal static readonly string TimesWonExact = "blackjack:times_won_exact";
        }
    }
}
