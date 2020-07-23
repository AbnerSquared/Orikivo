using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Services
{
    public class Leaderboard
    {
        public Leaderboard(LeaderboardFlag flag, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = flag;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public Leaderboard(string statId, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = LeaderboardFlag.Custom;
            StatId = statId;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public LeaderboardFlag Flag { get; } // If none is specified, just show the leaders of each flag
        public LeaderboardSort Sort { get; }

        // The STAT to compare.
        public string StatId { get; }

        // Example: User.Balance = 0
        public bool AllowEmptyValues { get; set; }

        public int PageSize { get; set; }

        private static string GetHeader(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Money => "> **Leaderboard - Wealth**",
                LeaderboardFlag.Debt => "> **Leaderboard - Debt**",
                LeaderboardFlag.Level => "> **Leaderboard - Experience**",
                LeaderboardFlag.Chips => "> **Leaderboard - Casino**",
                _ => "> **Leaderboard**"
            };
        }

        private static string GetHeaderQuote(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Default => "> *View the current pioneers of a specific category.*",
                LeaderboardFlag.Money => "> *These are the users that managed to beat all odds.*",
                LeaderboardFlag.Debt => "> *These are the users with enough debt to make a pool.*",
                LeaderboardFlag.Level => "> *These are the users dedicated to Orikivo.*",
                LeaderboardFlag.Chips => "> *These are the users that rule over the **Casino**.*",
                _ => ""
            };
        }

        private static string GetUserTitle(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Money => "The Wealthy",
                LeaderboardFlag.Debt => "The Cursed",
                LeaderboardFlag.Level => "The Experienced",
                LeaderboardFlag.Chips => "The Gambler",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetFlagSegment(LeaderboardFlag flag)
        {
            return flag switch
            {
                LeaderboardFlag.Money => "with 💸",
                LeaderboardFlag.Debt => "with 📃",
                LeaderboardFlag.Level => "at level",
                LeaderboardFlag.Chips => "with 🧩",
                _ => "INVALID_FLAG"
            };
        }

        private static readonly string LeaderFormat = "> **{0}**: **{1}** {2} **{3}**";
        private static readonly string UserFormat = "**{0}** ... {1} **{2}**";
        private static readonly string CustomFormat = "**{0}** ... **{1}**";

        // This is only on LeaderboardFlag.Default
        private static string WriteLeader(LeaderboardFlag flag, ArcadeUser user, bool allowEmptyValues = false)
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
                LeaderboardFlag.Money => string.Format(LeaderFormat, title, user.Username, segment, user.Balance.ToString("##,0")),
                LeaderboardFlag.Debt => string.Format(LeaderFormat, title, user.Username, segment, user.Debt.ToString("##,0")),
                LeaderboardFlag.Level => string.Format(LeaderFormat, title, user.Username, segment, WriteLevel(user)),
                LeaderboardFlag.Chips => string.Format(LeaderFormat, title, user.Username, segment, user.ChipBalance.ToString("##,0")),
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteUser(LeaderboardFlag flag, ArcadeUser user, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardFlag.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardFlag.Money => string.Format(UserFormat, user.Username, "💸", user.Balance.ToString("##,0")),
                LeaderboardFlag.Debt => string.Format(UserFormat, user.Username, "📃", user.Debt.ToString("##,0")),
                LeaderboardFlag.Level => string.Format(UserFormat, user.Username, "Level", WriteLevel(user)),
                LeaderboardFlag.Chips => string.Format(UserFormat, user.Username, "🧩", user.ChipBalance.ToString("##,0")),
                LeaderboardFlag.Custom => string.Format(CustomFormat, user.Username, user.GetStat(statId)),
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

            if (Flag != LeaderboardFlag.Custom)
            {
                leaderboard.Append(GetHeaderQuote(Flag));
            }
            else
            {
                leaderboard.Append($"> *Here are the users filtered for `{StatId}`*");

                if (Sort != LeaderboardSort.Least)
                    leaderboard.Append(".");
            }

            if (Sort == LeaderboardSort.Least && Flag != LeaderboardFlag.Default)
            {
                leaderboard.Append(" (ascending).");
            }

            leaderboard.AppendLine();

            if (Flag == LeaderboardFlag.Default)
            {
                leaderboard.AppendLine();
                leaderboard.AppendLine("**Leaders**");
                leaderboard.AppendLine(WriteLeader(LeaderboardFlag.Money, GetLeader(users, LeaderboardFlag.Money, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardFlag.Debt, GetLeader(users, LeaderboardFlag.Debt, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardFlag.Chips, GetLeader(users, LeaderboardFlag.Chips, Sort)));
                //leaderboard.Append(WriteLeader(LeaderboardFlag.Level, GetLeader(users, LeaderboardFlag.Level, Sort))); // Levels aren't implemented yet.
            }
            else
            {
                leaderboard.AppendLine();
                leaderboard.Append(WriteUsers(users, PageSize * page, PageSize, Flag, Sort, AllowEmptyValues, StatId));
            }

            return leaderboard.ToString();
        }

        private static ArcadeUser GetLeader(IEnumerable<ArcadeUser> users, LeaderboardFlag flag, LeaderboardSort sort)
            => SortUsers(users, flag, sort).First();

        private static IEnumerable<ArcadeUser> SortUsers(IEnumerable<ArcadeUser> users, LeaderboardFlag flag, LeaderboardSort sort, string statId = null)
        { 
            return sort switch
            {
                LeaderboardSort.Least => users.OrderBy(x => GetValue(x, flag, statId)),
                _ => users.OrderByDescending(x => GetValue(x, flag, statId))
            };
        }

        private static long GetValue(ArcadeUser user, LeaderboardFlag flag, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardFlag.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardFlag.Money => (long) user.Balance,
                LeaderboardFlag.Debt => (long) user.Debt,
                LeaderboardFlag.Level => user.Ascent * 100 + user.Level,
                LeaderboardFlag.Chips => (long) user.ChipBalance,
                LeaderboardFlag.Custom => user.GetStat(statId),
                _ => 0
            };
        }

        private static string WriteUsers(IEnumerable<ArcadeUser> users, int offset, int capacity, LeaderboardFlag flag, LeaderboardSort sort, bool allowEmptyValues = false, string statId = null)
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