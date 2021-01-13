using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia.Services
{
    public static class RankViewer
    {
        private static readonly int ElementLimit = 10;

        private static string GetSectionIcon(RankSection section)
        {
            return section switch
            {
                RankSection.Income => "🪙",
                RankSection.Experience => "🧠",
                RankSection.Quest => "🏟️",
                RankSection.Multiplayer => "🪃",
                RankSection.Casino => "🎯",
                _ => Icons.Unknown
            };
        }

        private static string GetSectionTitle(RankSection section)
        {
            return section switch
            {
                RankSection.Income => "The Wealthy",
                RankSection.Experience => "The Wise",
                RankSection.Quest => "The Challenger",
                RankSection.Multiplayer => "The Arcadian",
                RankSection.Casino => "The Predictor",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetDefaultHeader()
            => "📈 **Leaderboards**";

        private static string GetHeader(RankSection section)
        {
            if (section == 0)
                return GetDefaultHeader();

            return $"{GetSectionIcon(section)} Leaderboard: {section.ToString()}";
        }

        private static string GetSectionSubtitle(RankSection flag)
        {
            string subtitle = flag switch
            {
                RankSection.Income => "Stability in finance is what these members have mastered.",
                RankSection.Experience => "Those who seek experience shall be known.",
                RankSection.Quest => "Challenges that await these members are no issue.",
                RankSection.Multiplayer => "Gaming is their passion and it shows.",
                RankSection.Casino => "Risk and luck fuel these members.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(subtitle))
                return Format.Italics(subtitle);

            return subtitle;
        }
    }


    public class Leaderboard
    {
        private static readonly int ElementLimit = 10;

        private static string GetSectionIcon(RankSection section)
        {
            return section switch
            {
                RankSection.Income => "🪙",
                RankSection.Experience => "🧠",
                RankSection.Quest => "🏟️",
                RankSection.Multiplayer => "🪃",
                RankSection.Casino => "🎯",
                _ => Icons.Unknown
            };
        }

        private static string GetSectionTitle(RankSection section)
        {
            return section switch
            {
                RankSection.Income => "The Wealthy",
                RankSection.Experience => "The Wise",
                RankSection.Quest => "The Challenger",
                RankSection.Multiplayer => "The Arcadian",
                RankSection.Casino => "The Predictor",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetDefaultHeader()
            => "📈 **Leaderboards**";

        private static string GetHeader(RankSection section)
        {
            if (section == 0)
                return GetDefaultHeader();

            return $"{GetSectionIcon(section)} Leaderboard: {section.ToString()}";
        }

        private static string GetSectionSubtitle(RankSection flag)
        {
            string subtitle = flag switch
            {
                RankSection.Income => "Stability in finance is what these members have mastered.",
                RankSection.Experience => "Those who seek experience shall be known.",
                RankSection.Quest => "Challenges that await these members are no issue.",
                RankSection.Multiplayer => "Gaming is their passion and it shows.",
                RankSection.Casino => "Risk and luck fuel these members.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(subtitle))
                return Format.Italics(subtitle);

            return subtitle;
        }

        public Leaderboard(RankSection flag, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = flag;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public Leaderboard(string statId, LeaderboardSort sort, bool allowEmptyValues = false, int pageSize = 10)
        {
            Flag = RankSection.Custom;
            StatId = statId;
            Sort = sort;
            AllowEmptyValues = allowEmptyValues;
            PageSize = pageSize;
        }

        public RankSection Flag { get; } // If none is specified, just show the leaders of each flag
        public LeaderboardSort Sort { get; }

        // The STAT to compare.
        public string StatId { get; }

        // Example: User.Balance = 0
        public bool AllowEmptyValues { get; set; }

        public int PageSize { get; set; }

        

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

        

        private static string GetFlagSegment(RankSection flag)
        {
            return flag switch
            {
                RankSection.Income => $"with {Icons.Balance}",
                RankSection.Experience => $"with {Icons.Exp}",
                RankSection.Quest => "with", // {QUEST_POINT_ICON} 104
                RankSection.Multiplayer => "with", // 96 wins
                RankSection.Casino => $"with {Icons.Chips}",
                _ => "INVALID_FLAG"
            };
        }

        private static readonly string LeaderFormat = "> {0}: **{1}** {2} **{3}**";
        private static readonly string LeaderBaseFormat = "> {0}: **{1}** {2} {3}";
        private static readonly string UserFormat = "**{0}** ... {1} **{2}**";
        private static readonly string CustomBaseFormat = "**{0}** ... {1}";
        private static readonly string CustomFormat = "**{0}** ... **{1}**";

        // This is only on LeaderboardFlag.Default
        private static string WriteLeader(RankSection flag, ArcadeUser user, bool allowEmptyValues = false)
        {
            var title = $"{GetSectionIcon(flag)} {GetSectionTitle(flag)}";
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
                RankSection.Income => string.Format(LeaderFormat, title, user.Username, segment, $"{Icons.Balance} **{user.GetVar(Vars.MonthlyIncome).ToString("##,0")}**"),
                RankSection.Experience => string.Format(LeaderFormat, title, user.Username, segment, $"{Icons.Exp} **{user.GetVar(Vars.MonthlyExp).ToString("##,0")} XP**"),
                RankSection.Quest => string.Format(LeaderFormat, title, user.Username, segment, $"**{user.GetVar(Vars.MonthlyQuests).ToString("##,0")} QP**"),
                RankSection.Multiplayer => string.Format(LeaderFormat, title, user.Username, segment, $"**{user.GetVar(Vars.MonthlyArcade).ToString("##,0")} AP**"),
                RankSection.Casino => string.Format(LeaderBaseFormat, title, user.Username, segment, $"{Icons.Chips} **{user.GetVar(Vars.MonthlyCasino).ToString("##,0")}**"),
                _ => "INVALID_FLAG"
            };
        }

        private static string WriteUser(RankSection flag, ArcadeUser user, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == RankSection.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                RankSection.Income => string.Format(UserFormat, user.Username, Icons.Balance, $"**{user.GetVar(Vars.MonthlyIncome).ToString("##,0")}**"),
                RankSection.Experience => string.Format(UserFormat, user.Username, Icons.Exp, $"**{user.GetVar(Vars.MonthlyExp).ToString("##,0")} XP**"),
                RankSection.Quest => string.Format(CustomBaseFormat, user.Username, $"**{user.GetVar(Vars.MonthlyQuests).ToString("##,0")} QP**"),
                RankSection.Multiplayer => string.Format(CustomBaseFormat, user.Username, $"**{user.GetVar(Vars.MonthlyArcade).ToString("##,0")} AP**"),
                RankSection.Casino => string.Format(CustomBaseFormat, user.Username, $"{Icons.Chips} **{user.GetVar(Vars.MonthlyCasino).ToString("##,0")}**"),
                RankSection.Custom => string.Format(CustomFormat, user.Username, user.GetVar(statId)),
                _ => "INVALID_FLAG"
            };
        }

        public string Write(ArcadeUser user, in IEnumerable<ArcadeUser> users, int page = 0)
        {
            var leaderboard = new StringBuilder();
            bool allowTooltips = user.Config.Tooltips;

            if (allowTooltips && Flag == RankSection.Default)
            {
                leaderboard
                    .AppendLine(Format.Tooltip("Type `leaderboard <category | stat>` to view a specific leaderboard."))
                    .AppendLine();
            }

            leaderboard.AppendLine(GetHeader(Flag));

            if (Flag == RankSection.Default)
            {
                DateTime now = DateTime.UtcNow;
                int year = now.Year;
                int month = now.Month;
                int day = now.Day;
                int daysInMonth = DateTime.DaysInMonth(year, month);
                int remainder = daysInMonth - day;
                string remaining = $"**{remainder:##,0}** {Format.TryPluralize("day", remainder)} until a new cycle";
                string quote = $"> **{Format.GetMonthName(month)} {year}** ({remaining})";
                leaderboard.Append(quote);
            }
            else if (Flag != RankSection.Custom)
            {
                leaderboard.Append(GetSectionSubtitle(Flag));
            }
            else
            {
                leaderboard.Append($"> *Here are the leaders from the following stat: `{StatId}`*");

                if (Sort != LeaderboardSort.Least)
                    leaderboard.Append(".");
            }

            if (Sort == LeaderboardSort.Least && Flag != RankSection.Default)
            {
                leaderboard.Append(" (**Ascending**).");
            }

            leaderboard.AppendLine();

            if (Flag == RankSection.Default)
            {
                leaderboard.AppendLine();
                leaderboard.AppendLine("> **Categories**"); // Find a better name than categories??
                leaderboard.AppendJoin(" ", EnumUtils.GetValues<RankSection>().Where(x => !x.EqualsAny(RankSection.Default, RankSection.Custom)).Select(x => $"`{x.ToString().ToLower()}`").OrderBy(x => x[1..]));
                leaderboard.AppendLine().AppendLine();
                leaderboard.AppendLine("> **Leaders**");
                leaderboard.AppendLine(WriteLeader(RankSection.Income, GetLeader(users, RankSection.Income, Sort)));
                leaderboard.AppendLine(WriteLeader(RankSection.Experience, GetLeader(users, RankSection.Experience, Sort)));
                leaderboard.AppendLine(WriteLeader(RankSection.Multiplayer, GetLeader(users, RankSection.Multiplayer, Sort)));
                leaderboard.AppendLine(WriteLeader(RankSection.Casino, GetLeader(users, RankSection.Casino, Sort)));
                leaderboard.Append(WriteLeader(RankSection.Quest, GetLeader(users, RankSection.Quest, Sort))); // Levels aren't implemented yet.
            }
            else
            {
                leaderboard.AppendLine();
                leaderboard.Append(WriteUsers(users, PageSize * page, PageSize, Flag, Sort, AllowEmptyValues, StatId));
            }

            return leaderboard.ToString();
        }

        private static ArcadeUser GetLeader(IEnumerable<ArcadeUser> users, RankSection flag, LeaderboardSort sort)
            => SortUsers(users, flag, sort).First();

        private static IEnumerable<ArcadeUser> SortUsers(IEnumerable<ArcadeUser> users, RankSection flag, LeaderboardSort sort, string statId = null)
        { 
            return sort switch
            {
                LeaderboardSort.Least => users.OrderBy(x => GetValue(x, flag, statId)),
                _ => users.OrderByDescending(x => GetValue(x, flag, statId))
            };
        }

        private static long GetValue(ArcadeUser user, RankSection flag, string statId = null)
        {
            if (string.IsNullOrWhiteSpace(statId) && flag == RankSection.Custom)
                throw new ArgumentException("Cannot use a custom flag if the stat is unspecified.");

            return flag switch
            {
                RankSection.Income => user.GetVar(Vars.MonthlyIncome),
                RankSection.Experience => user.GetVar(Vars.MonthlyExp),
                RankSection.Quest => user.GetVar(Vars.MonthlyQuests), // user.Ascent * 100 + user.Level,
                RankSection.Multiplayer => user.GetVar(Vars.MonthlyArcade),
                RankSection.Casino => user.GetVar(Vars.MonthlyCasino), // MeritHelper.GetScore(user),
                RankSection.Custom => user.GetVar(statId),
                _ => 0
            };
        }

        private static string WriteUsers(in IEnumerable<ArcadeUser> users, int offset, int capacity, RankSection flag, LeaderboardSort sort, bool allowEmptyValues = false, string statId = null)
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
                return "This leaderboard is empty. You could be the first!";

            return result.ToString();
        }
    }
}