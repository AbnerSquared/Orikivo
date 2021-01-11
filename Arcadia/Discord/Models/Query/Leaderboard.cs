using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

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
            string icon = GetLeaderIcon(flag);
            return flag switch
            {
                LeaderboardQuery.Income => $"> {icon} **Leaderboard: Income**",
                LeaderboardQuery.Experience => $"> {icon} **Leaderboard: Experience**",
                LeaderboardQuery.Quest => $"> {icon} **Leaderboard: Quest**",
                LeaderboardQuery.Multiplayer => $"> {icon} **Leaderboard: Multiplayer**",
                LeaderboardQuery.Casino => $"> {icon} **Leaderboard: Casino**",
                _ => "> 📈 **Leaderboards**"
            };
        }

        private static string GetHeaderQuote(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Default => "> View the leaders of the five primary categories.",
                LeaderboardQuery.Income => "> *Stability in finance is what these members have mastered.*",
                LeaderboardQuery.Experience => "> *Those who seek experience shall be known.*",
                LeaderboardQuery.Quest => "> *Challenges that await these members are no issue.*",
                LeaderboardQuery.Multiplayer => "> *Gaming is their passion and it shows.*",
                LeaderboardQuery.Casino => "> *Risk and luck fuel these members.*",
                _ => ""
            };
        }

        public static int GetPosition(in IEnumerable<ArcadeUser> users, ArcadeUser user, string statId)
        {
            if (users.All(x => x.Id != user.Id))
                throw new Exception("Expected to find user in user collection");

            IOrderedEnumerable<ArcadeUser> sorted = users.OrderByDescending(x => x.GetVar(statId));

            int position = 0;
            foreach (ArcadeUser account in sorted)
            {
                if (account.Id == user.Id)
                    break;

                position++;
            }

            return position + 1;
        }

        private static string GetLeaderIcon(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Income => "🪙",
                LeaderboardQuery.Experience => "🧠",
                LeaderboardQuery.Quest => "🏟️",
                LeaderboardQuery.Multiplayer => "🪃",
                LeaderboardQuery.Casino => "🎯",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetUserTitle(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Income => "The Wealthy",
                LeaderboardQuery.Experience => "The Wise",
                LeaderboardQuery.Quest => "The Challenger",
                LeaderboardQuery.Multiplayer => "The Arcadian",
                LeaderboardQuery.Casino => "The Predictor",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetFlagSegment(LeaderboardQuery flag)
        {
            return flag switch
            {
                LeaderboardQuery.Income => $"with {Icons.Balance}",
                LeaderboardQuery.Experience => $"with {Icons.Exp}",
                LeaderboardQuery.Quest => "with", // {QUEST_POINT_ICON} 104
                LeaderboardQuery.Multiplayer => "with", // 96 wins
                LeaderboardQuery.Casino => $"with {Icons.Chips}",
                _ => "INVALID_FLAG"
            };
        }

        private static readonly string LeaderFormat = "> {0}: **{1}** {2} **{3}**";
        private static readonly string LeaderBaseFormat = "> {0}: **{1}** {2} {3}";
        private static readonly string UserFormat = "**{0}** ... {1} **{2}**";
        private static readonly string CustomBaseFormat = "**{0}** ... {1}";
        private static readonly string CustomFormat = "**{0}** ... **{1}**";

        // This is only on LeaderboardFlag.Default
        private static string WriteLeader(LeaderboardQuery flag, ArcadeUser user, bool allowEmptyValues = false)
        {
            var title = $"{GetLeaderIcon(flag)} {GetUserTitle(flag)}";
            var segment = GetFlagSegment(flag);


            if (!allowEmptyValues)
            {
                if (GetValue(user, flag) == 0)
                {
                    return $"> {title}: **Nobody!**";
                }
            }

            return flag switch
            {
                LeaderboardQuery.Income => string.Format(LeaderFormat, title, user.Username, segment, user.GetVar(Vars.MonthlyIncome).ToString("##,0")),
                LeaderboardQuery.Experience => string.Format(LeaderFormat, title, user.Username, segment, user.GetVar(Vars.MonthlyExp).ToString("##,0")),
                LeaderboardQuery.Quest => string.Format(LeaderFormat, title, user.Username, segment, user.GetVar(Vars.MonthlyQuests).ToString("##,0")),
                LeaderboardQuery.Multiplayer => string.Format(LeaderFormat, title, user.Username, segment, user.GetVar(Vars.MonthlyArcade).ToString("##,0")),
                LeaderboardQuery.Casino => string.Format(LeaderBaseFormat, title, user.Username, segment, user.GetVar(Vars.MonthlyCasino).ToString("##,0")), // $"**{MeritHelper.GetScore(user)}**m")
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteUser(LeaderboardQuery flag, ArcadeUser user, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == LeaderboardQuery.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                LeaderboardQuery.Income => string.Format(UserFormat, user.Username, "💸", user.Balance.ToString("##,0")),
                LeaderboardQuery.Experience => string.Format(UserFormat, user.Username, "📃", user.Debt.ToString("##,0")),
                LeaderboardQuery.Quest => string.Format(UserFormat, user.Username, "Level", WriteLevel(user)),
                LeaderboardQuery.Multiplayer => string.Format(UserFormat, user.Username, "🧩", user.ChipBalance.ToString("##,0")),
                LeaderboardQuery.Casino => string.Format(CustomBaseFormat, user.Username, $"**{MeritHelper.GetScore(user)}**m"),
                LeaderboardQuery.Custom => string.Format(CustomFormat, user.Username, user.GetVar(statId)),
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

        private static string GetMonthName(int month)
        {
            return month switch
            {
                1 => "January",
                2 => "February",
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => throw new ArgumentException("The specified month is out of the possible range")
            };
        }

        public string Write(ArcadeUser user, in IEnumerable<ArcadeUser> users, int page = 0)
        {
            var leaderboard = new StringBuilder();
            bool allowTooltips = user.Config.Tooltips;

            if (allowTooltips && Flag == LeaderboardQuery.Default)
            {
                leaderboard
                    .AppendLine(Format.Tooltip("Type `leaderboard <category | stat>` to view a specific leaderboard."))
                    .AppendLine();
            }

            leaderboard.AppendLine(GetHeader(Flag));

            if (Flag == LeaderboardQuery.Default)
            {
                DateTime now = DateTime.UtcNow;
                int year = now.Year;
                int month = now.Month;
                int day = now.Day;
                int daysInMonth = DateTime.DaysInMonth(year, month);
                int remainder = daysInMonth - day;
                string remaining = $"**{remainder:##,0}** {Format.TryPluralize("day", remainder)} until a new cycle";
                string quote = $"> **{GetMonthName(month)} {year}** ({remaining})";
                leaderboard.Append(quote);
            }
            if (Flag != LeaderboardQuery.Custom)
            {
                leaderboard.Append(GetHeaderQuote(Flag));
            }
            else
            {
                leaderboard.Append($"> *Here are the leaders from the following stat: `{StatId}`*");

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
                leaderboard.AppendLine("> **Categories**"); // Find a better name than categories??
                leaderboard.AppendJoin(" ", EnumUtils.GetValues<LeaderboardQuery>().Where(x => !x.EqualsAny(LeaderboardQuery.Default, LeaderboardQuery.Custom)).Select(x => $"`{x.ToString().ToLower()}`").OrderBy(x => x[1..]));
                leaderboard.AppendLine().AppendLine();
                leaderboard.AppendLine("> **Leaders**");
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Income, GetLeader(users, LeaderboardQuery.Income, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Experience, GetLeader(users, LeaderboardQuery.Experience, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Multiplayer, GetLeader(users, LeaderboardQuery.Multiplayer, Sort)));
                leaderboard.AppendLine(WriteLeader(LeaderboardQuery.Casino, GetLeader(users, LeaderboardQuery.Casino, Sort)));
                leaderboard.Append(WriteLeader(LeaderboardQuery.Quest, GetLeader(users, LeaderboardQuery.Quest, Sort))); // Levels aren't implemented yet.
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
                LeaderboardQuery.Income => user.GetVar(Vars.MonthlyIncome),
                LeaderboardQuery.Experience => user.GetVar(Vars.MonthlyExp),
                LeaderboardQuery.Quest => user.GetVar(Vars.MonthlyQuests), // user.Ascent * 100 + user.Level,
                LeaderboardQuery.Multiplayer => user.GetVar(Vars.MonthlyArcade),
                LeaderboardQuery.Casino => user.GetVar(Vars.MonthlyCasino), // MeritHelper.GetScore(user),
                LeaderboardQuery.Custom => user.GetVar(statId),
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