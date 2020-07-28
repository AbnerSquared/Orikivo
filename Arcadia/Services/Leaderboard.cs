using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Services
{
    public class Leaderboard
    {
        public Leaderboard(LeaderboardQuery flag, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = flag;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public Leaderboard(string statId, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = LeaderboardQuery.Custom;
            StatId = statId;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public LeaderboardQuery Flag { get; } // If none is specified, just show the leaders of each flag
        public LeaderboardSort Sort { get; }

        // The STAT to compare.
        public string StatId { get; }

        // Example: User.Balance = 0
        public bool AllowEmptyValues { get; set; }

        public int PageSize { get; set; }

        private static string GetHeader(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Money => "> 📈 **Leaderboard: Wealth**",
                LeaderboardQuery.Debt => "> 📈 **Leaderboard: Debt**",
                LeaderboardQuery.Level => "> 📈 **Leaderboard: Experience**",
                LeaderboardQuery.Chips => "> 📈 **Leaderboard: Casino**",
                _ => "> 📈 **Leaderboard**"
            };
        }

        private static string GetHeaderQuote(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Default => "> View the current pioneers of a specific category.",
                LeaderboardQuery.Money => "> *These are the users that managed to beat all odds.*",
                LeaderboardQuery.Debt => "> *These are the users with enough debt to make a pool.*",
                LeaderboardQuery.Level => "> *These are the users dedicated to Orikivo.*",
                LeaderboardQuery.Chips => "> *These are the users that rule over the **Casino**.*",
                _ => ""
            };
        }

        private static string GetUserTitle(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Money => "The Wealthy",
                LeaderboardQuery.Debt => "The Cursed",
                LeaderboardQuery.Level => "The Experienced",
                LeaderboardQuery.Chips => "The Gambler",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetFlagSegment(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Money => "with 💸",
                LeaderboardQuery.Debt => "with 📃",
                LeaderboardQuery.Level => "at level",
                LeaderboardQuery.Chips => "with 🧩",
                _ => "INVALID_FLAG"
            };
        }

        private static readonly string LeaderFormat = "> **{0}**: **{1}** {2} **{3}**";
        private static readonly string UserFormat = "**{0}** ... {1} **{2}**";
        private static readonly string CustomFormat = "**{0}** ... **{1}**";

        // This is only on LeaderboardFlag.Default
        private static string WriteLeader(LeaderboardQuery flag, ArcadeUser user, bool allowEmptyValues = false)
        {
            var title = GetUserTitle(flag);
            var segment = GetFlagSegment(flag);


            if (!allowEmptyValues)
            {
                if (GetValue(user, flag) == 0)
                {
                    return $"> **{title}**: **Nobody!**";
                }
            }

            return flag switch
            {
                LeaderboardQuery.Money => string.Format(LeaderFormat, title, user.Username, segment, user.Balance.ToString("##,0")),
                LeaderboardQuery.Debt => string.Format(LeaderFormat, title, user.Username, segment, user.Debt.ToString("##,0")),
                LeaderboardQuery.Level => string.Format(LeaderFormat, title, user.Username, segment, WriteLevel(user)),
                LeaderboardQuery.Chips => string.Format(LeaderFormat, title, user.Username, segment, user.ChipBalance.ToString("##,0")),
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteUser(LeaderboardQuery flag, ArcadeUser user, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardQuery.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardQuery.Money => string.Format(UserFormat, user.Username, "💸", user.Balance.ToString("##,0")),
                LeaderboardQuery.Debt => string.Format(UserFormat, user.Username, "📃", user.Debt.ToString("##,0")),
                LeaderboardQuery.Level => string.Format(UserFormat, user.Username, "Level", WriteLevel(user)),
                LeaderboardQuery.Chips => string.Format(UserFormat, user.Username, "🧩", user.ChipBalance.ToString("##,0")),
                LeaderboardQuery.Custom => string.Format(CustomFormat, user.Username, user.GetStat(statId)),
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteLevel(ArcadeUser user)
        {
            string level = $"{user.Level}";

            if (user.Ascent > 0)
                level = $"{user.Ascent}." + level;

            return level;
        }

        public string Write(in IEnumerable<ArcadeUser> users, int page = 0)
        {
            var leaderboard = new StringBuilder();

            leaderboard.AppendLine(GetHeader(Flag));

            if (Flag != LeaderboardQuery.Custom)
            {
                leaderboard.Append(GetHeaderQuote(Flag));
            }
            else
            {
                leaderboard.Append($"> *Here are the users filtered for `{StatId}`*");

                if (Sort != LeaderboardSort.Least)
                    leaderboard.Append(".");
            }

            if (Sort == LeaderboardSort.Least && Flag != LeaderboardQuery.Default)
            {
                leaderboard.Append(" (**Ascending**).");
            }

            leaderboard.AppendLine();

            if (Flag == LeaderboardQuery.Default)
            {
                leaderboard.AppendLine();
                leaderboard.AppendLine("**Leaders**");
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Money, GetLeader(users, LeaderboardQuery.Money, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Debt, GetLeader(users, LeaderboardQuery.Debt, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Chips, GetLeader(users, LeaderboardQuery.Chips, Sort)));
                //leaderboard.Append(WriteLeader(LeaderboardFlag.Level, GetLeader(users, LeaderboardFlag.Level, Sort))); // Levels aren't implemented yet.
            }
            else
            {
                leaderboard.AppendLine();
                leaderboard.Append(WriteUsers(users, PageSize * page, PageSize, Flag, Sort, AllowEmptyValues, StatId));
            }

            return leaderboard.ToString();
        }

        private static ArcadeUser GetLeader(IEnumerable<ArcadeUser> users, LeaderboardQuery flag, LeaderboardSort sort)
            => SortUsers(users, flag, sort).First();

        private static IEnumerable<ArcadeUser> SortUsers(IEnumerable<ArcadeUser> users, LeaderboardQuery flag, LeaderboardSort sort, string statId = null)
        { 
            return sort switch
            {
                LeaderboardSort.Least => users.OrderBy(x => GetValue(x, flag, statId)),
                _ => users.OrderByDescending(x => GetValue(x, flag, statId))
            };
        }

        private static long GetValue(ArcadeUser user, LeaderboardQuery flag, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardQuery.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardQuery.Money => (long) user.Balance,
                LeaderboardQuery.Debt => (long) user.Debt,
                LeaderboardQuery.Level => user.Ascent * 100 + user.Level,
                LeaderboardQuery.Chips => (long) user.ChipBalance,
                LeaderboardQuery.Custom => user.GetStat(statId),
                _ => 0
            };
        }

        private static string WriteUsers(in IEnumerable<ArcadeUser> users, int offset, int capacity, LeaderboardQuery flag, LeaderboardSort sort, bool allowEmptyValues = false, string statId = null)
        {
            if (users.Count() <= offset)
                throw new ArgumentException("The specified offset is larger than the amount of users specified.");

            var result = new StringBuilder();

            // The indexing is done this way, so that it doesn't have to be at that exact amount.
            var i = 0;
            foreach (ArcadeUser user in SortUsers(users.Skip(offset), flag, sort, statId))
            {
                long value = GetValue(user, flag, statId);

                if (!allowEmptyValues && value == 0)
                    continue;

                result.AppendLine(WriteUser(flag, user, statId));
                i++;
            }

            if (i == 0)
                return "No users were provided for this leaderboard.";

            return result.ToString();
        }
    }
}