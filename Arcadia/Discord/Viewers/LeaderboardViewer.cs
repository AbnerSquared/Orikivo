using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Services
{
    public static class LeaderboardViewer
    {
        private static readonly int PageLength = 8;

        private static readonly string DefaultIcon = "🎖️";

        private static string GetSectionIcon(RankingType section)
        {
            return section switch
            {
                RankingType.Income => "🪙",
                RankingType.Experience => "🧠",
                RankingType.Quest => "🏟️",
                RankingType.Multiplayer => "🪃",
                RankingType.Casino => "🎯",
                _ => Icons.Unknown
            };
        }

        private static string GetSectionName(RankingType section)
        {
            return section switch
            {
                RankingType.Income => "The Field of Wealth",
                RankingType.Experience => "Wisdom's Canyon",
                RankingType.Quest => "Challenger's Approach",
                RankingType.Multiplayer => "The Arcade",
                RankingType.Casino => "The Prediction Pool",
                _ => "REDACTED"
            };
        }

        private static string GetSectionCall(RankingType section)
        {
            return section switch
            {
                RankingType.Income => "The Wealthy",
                RankingType.Experience => "The Wise",
                RankingType.Quest => "The Challenger",
                RankingType.Multiplayer => "The Arcadian",
                RankingType.Casino => "The Predictor",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetSectionSubtitle(RankingType flag)
        {
            string subtitle = flag switch
            {
                RankingType.Income => "Stability in finance is what these members have mastered.",
                RankingType.Experience => "Those who seek experience shall be known.",
                RankingType.Quest => "Challenges that await these members are no issue.",
                RankingType.Multiplayer => "Gaming is their passion and it shows.",
                RankingType.Casino => "Risk and luck fuel these members.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(subtitle))
                return $"\"{subtitle}\"";

            return subtitle;
        }

        private static string GetCycleSubtitle()
        {
            DateTime now = DateTime.UtcNow;
            int year = now.Year;
            int month = now.Month;
            int day = now.Day;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int remainder = daysInMonth - day;

            string remaining = remainder == 0
                ? $"A new cycle begins in {Format.Countdown(TimeSpan.FromHours(24).Subtract(TimeSpan.FromHours(now.Hour)))}"
                : $"**{remainder:##,0} {Format.TryPluralize("Day", remainder)}** until a new cycle";
            return $"**{Format.GetMonthName(month)} {year}** ({remaining})";
        }

        private static long GetRawValue(ArcadeUser user, string statId)
            => user.GetVar(statId);

        private static long GetRawValue(ArcadeUser user, RankingType section)
            => GetRawValue(user, GetSectionId(section));

        public static string GetSectionId(RankingType section)
        {
            return section switch
            {
                RankingType.Income => Vars.MonthlyIncome,
                RankingType.Experience => Vars.MonthlyExp,
                RankingType.Quest => Vars.MonthlyQuests,
                RankingType.Multiplayer => Vars.MonthlyArcade,
                RankingType.Casino => Vars.MonthlyCasino,
                _ => throw new ArgumentException("An unknown leaderboard ranking section was specified")
            };
        }

        public static int FindPosition(in IEnumerable<ArcadeUser> users, ArcadeUser user, string statId)
        {
            if (users.All(x => x.Id != user.Id))
                throw new Exception("Expected to find user in user collection");

            List<ArcadeUser> sorted = users.OrderByDescending(x => x.GetVar(statId)).ToList();
            return sorted.FindIndex(x => x.Id == user.Id) + 1;
        }

        private static string WriteLeader(ArcadeUser user, RankingType section, bool allowEmptyValues = false)
        {
            if (user == null || !allowEmptyValues && GetRawValue(user, section) == 0)
                return "";

            return $"> Leading: **{user.Username}** with {Var.WriteValue(user, GetSectionId(section))}";
        }

        private static IEnumerable<ArcadeUser> SortUsers(IEnumerable<ArcadeUser> users, string statId, bool allowEmptyValues = false)
        {
            if (!allowEmptyValues)
                return users.Where(x => GetRawValue(x, statId) != 0).OrderByDescending(x => GetRawValue(x, statId));

            return users.OrderByDescending(x => GetRawValue(x, statId));
        }

        private static string WriteUser(ArcadeUser user, RankingType section)
            => WriteUser(user, GetSectionId(section));

        private static string WriteUser(ArcadeUser user, string statId)
            => $"> **{user.Username}**: {Var.WriteValue(user, statId)}";

        private static ArcadeUser GetLeader(IEnumerable<ArcadeUser> users, string statId)
            => SortUsers(users, statId).FirstOrDefault();

        private static string DrawSectionPreviews(IEnumerable<ArcadeUser> users)
        {
            return new StringBuilder()
                .AppendJoin("\n\n", EnumUtils.GetValues<RankingType>().Select(x => DrawSection(x, users)))
                .ToString();
        }

        private static string DrawSection(RankingType section, IEnumerable<ArcadeUser> users)
        {
            var result = new StringBuilder();

            result.AppendLine($"> `{section.ToString().ToLower()}`")
                .Append($"> {GetSectionIcon(section)} **{GetSectionName(section)}**");

            ArcadeUser leader = GetLeader(users, GetSectionId(section));

            if (leader != null)
                result.AppendLine().Append(WriteLeader(leader, section));

            return result.ToString();
        }

        private static string GetPositionSubtitle(ArcadeUser user, IEnumerable<ArcadeUser> users, string statId)
        {
            return $"Position: **{FindPosition(users, user, statId):##,0}** of **{users.Count():##,0}**";
        }

        private static string WriteUsers(in IEnumerable<ArcadeUser> sorted, int page, string statId)
        {
            int pageCount = Paginate.GetPageCount(sorted.Count(), PageLength);
            page = Paginate.ClampIndex(page, pageCount);

            return string.Join("\n", Paginate.GroupAt(sorted, page, PageLength).Select(x => WriteUser(x, statId)));
        }

        private static string ViewQuery(ArcadeUser user, in IEnumerable<ArcadeUser> users, string query, int page = 0)
        {
            bool isSection = Enum.TryParse(query, true, out RankingType section);
            string statId = isSection ? GetSectionId(section) : query;

            if (!isSection && !Var.IsType(statId, VarType.Stat))
                return Format.Warning("The variable that you specify is not a stat.");

            IEnumerable<ArcadeUser> sorted = SortUsers(users, statId);
            int pageCount = Paginate.GetPageCount(sorted.Count(), PageLength);
            page = Paginate.ClampIndex(page, pageCount);

            var result = new TextBody();

            if (!isSection && Check.NotNullOrEmpty(sorted))
                result.AppendTip("This leaderboard is unranked.");

            string counter = Format.PageCount(page + 1, pageCount, "({0})", false);

            var header = new Header
            {
                Title = isSection ? GetSectionName(section) : "Leaderboard",
                Icon = isSection ? GetSectionIcon(section) : DefaultIcon,
                Extra = isSection ? counter : $"(for `{(query.Replace("`", "\\`"))}`) {counter}",
                Subtitle = isSection ? GetSectionSubtitle(section) : !Check.NotNullOrEmpty(sorted) ? "This stat isn't stored on anyone." : GetPositionSubtitle(user, sorted, statId)
            };

            if (!Check.NotNullOrEmpty(sorted))
            {
                if (!isSection)
                {
                    header.Title = "Leaderboards";
                    header.Extra = null;
                }

                header.Subtitle = isSection ? "This section is empty." : "This stat isn't stored on anyone.";
            }
            else
            {
                result.WithSection(null, WriteUsers(sorted, page, statId));
            }

            result.WithHeader(header);

            return result.Build(user.Config.Tooltips);
        }

        public static string View(ArcadeUser user, in IEnumerable<ArcadeUser> users, int page = 0, string query = null)
        {
            if (!string.IsNullOrWhiteSpace(query))
                return ViewQuery(user, users, query, page);

            var result = new TextBody();

            result.WithHeader("Leaderboards", icon: DefaultIcon, subtitle: GetCycleSubtitle());

            result.AppendTip("Type `leaderboard <section | stat>` to view a specific leaderboard.");

            result.WithSection(null, DrawSectionPreviews(users));
                
            return result.Build(user.Config.Tooltips);
        }
    }
}
