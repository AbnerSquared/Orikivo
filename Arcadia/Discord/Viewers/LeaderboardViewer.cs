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

        private static readonly string DefaultIcon = "📈";

        private static string GetSectionIcon(LeaderSection section)
        {
            return section switch
            {
                LeaderSection.Income => "🪙",
                LeaderSection.Experience => "🧠",
                LeaderSection.Quest => "🏟️",
                LeaderSection.Multiplayer => "🪃",
                LeaderSection.Casino => "🎯",
                _ => Icons.Unknown
            };
        }

        private static string GetSectionTitle(LeaderSection section)
        {
            return section switch
            {
                LeaderSection.Income => "The Wealthy",
                LeaderSection.Experience => "The Wise",
                LeaderSection.Quest => "The Challenger",
                LeaderSection.Multiplayer => "The Arcadian",
                LeaderSection.Casino => "The Predictor",
                _ => "INVALID_FLAG"
            };
        }

        private static string GetSectionSubtitle(LeaderSection flag)
        {
            string subtitle = flag switch
            {
                LeaderSection.Income => "Stability in finance is what these members have mastered.",
                LeaderSection.Experience => "Those who seek experience shall be known.",
                LeaderSection.Quest => "Challenges that await these members are no issue.",
                LeaderSection.Multiplayer => "Gaming is their passion and it shows.",
                LeaderSection.Casino => "Risk and luck fuel these members.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(subtitle))
                return Format.Italics(subtitle);

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
            string remaining = $"**{remainder:##,0}** {Format.TryPluralize("day", remainder)} until a new cycle";
            return $"**{Format.GetMonthName(month)} {year}** ({remaining})";
        }

        private static long GetRawValue(ArcadeUser user, string statId)
            => user.GetVar(statId);

        private static long GetRawValue(ArcadeUser user, LeaderSection section)
            => GetRawValue(user, GetSectionId(section));

        public static string GetSectionId(LeaderSection section)
        {
            return section switch
            {
                LeaderSection.Income => Vars.MonthlyIncome,
                LeaderSection.Experience => Vars.MonthlyExp,
                LeaderSection.Quest => Vars.MonthlyQuests,
                LeaderSection.Multiplayer => Vars.MonthlyArcade,
                LeaderSection.Casino => Vars.MonthlyCasino,
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

        private static string WriteLeader(ArcadeUser user, LeaderSection section, bool allowEmptyValues = false)
        {
            var title = $"{GetSectionIcon(section)} {GetSectionTitle(section)}";

            if (!allowEmptyValues && GetRawValue(user, section) == 0)
                return $"> {title}: **Nobody!**";

            return $"> {title}: **{user.Username}** with {Var.WriteValue(user, GetSectionId(section))}";
        }

        private static IEnumerable<ArcadeUser> SortUsers(IEnumerable<ArcadeUser> users, string statId, bool allowEmptyValues = false)
        {
            if (!allowEmptyValues)
                return users.Where(x => GetRawValue(x, statId) != 0).OrderByDescending(x => GetRawValue(x, statId));

            return users.OrderByDescending(x => GetRawValue(x, statId));
        }

        private static string WriteUser(ArcadeUser user, LeaderSection section)
            => WriteUser(user, GetSectionId(section));

        private static string WriteUser(ArcadeUser user, string statId)
            => $"**{user.Username}**: {Var.WriteValue(user, statId)}";

        private static ArcadeUser GetLeader(IEnumerable<ArcadeUser> users, string statId)
            => SortUsers(users, statId).First();

        private static string PreviewLeaders(IEnumerable<ArcadeUser> users)
        {
            return new StringBuilder()
                .AppendJoin("\n", EnumUtils.GetValues<LeaderSection>().Select(x => WriteLeader(GetLeader(users, GetSectionId(x)), x)))
                .ToString();
        }

        private static string PreviewSections()
        {
            return new StringBuilder()
                .AppendJoin(" ", EnumUtils.GetValues<LeaderSection>().Select(x => $"`{x.ToString().ToLower()}`").OrderBy(x => x[1..]))
                .ToString();
        }

        private static string GetPositionSubtitle(ArcadeUser user, IEnumerable<ArcadeUser> users, string statId)
        {
            return $"Position: **{FindPosition(users, user, statId):##,0}** of **{users.Count():##,0}**";
        }

        private static string WriteUsers(in IEnumerable<ArcadeUser> sorted, int page, string statId)
        {
            if (!Check.NotNullOrEmpty(sorted))
                return "This leaderboard is empty.";

            int pageCount = Paginate.GetPageCount(sorted.Count(), PageLength);
            page = Paginate.ClampIndex(page, pageCount);

            return string.Join("\n", Paginate.GroupAt(sorted, page, PageLength).Select(x => WriteUser(x, statId)));
        }

        private static string ViewQuery(ArcadeUser user, in IEnumerable<ArcadeUser> users, string query, int page = 0)
        {
            bool isSection = Enum.TryParse(query, true, out LeaderSection section);
            string statId = isSection ? GetSectionId(section) : query;

            IEnumerable<ArcadeUser> sorted = SortUsers(users, statId);
            int pageCount = Paginate.GetPageCount(sorted.Count(), PageLength);
            page = Paginate.ClampIndex(page, pageCount);

            var result = new TextBody();

            if (!isSection)
                result.AppendTip("This leaderboard is unranked.");

            string counter = Format.PageCount(page, pageCount, "({0})", false);

            var header = new Header
            {
                Title = isSection ? "Leaderboard" : "Leaderboards:",
                Icon = isSection ? GetSectionIcon(section) : DefaultIcon,
                Extra = isSection ? $"`{Format.Sanitize(query)}` {counter}" : counter,
                Subtitle = isSection ? GetSectionSubtitle(section) : GetPositionSubtitle(user, sorted, GetSectionId(section))
            };

            if (isSection)
                header.Group = section.ToString();

            result
                .WithHeader(header)
                .WithSection(null, WriteUsers(sorted, page, statId));

            return result.Build(user.Config.Tooltips);
        }

        public static string View(ArcadeUser user, in IEnumerable<ArcadeUser> users, int page = 0, string query = null)
        {
            if (!string.IsNullOrWhiteSpace(query))
                return ViewQuery(user, users, query, page);

            var result = new TextBody();

            result.WithHeader("");

            result.AppendTip("Type `leaderboard <section | stat>` to view a specific leaderboard.");

            result.WithSection("**Sections**", PreviewSections())
                .WithSection("**Leaders**", PreviewLeaders(users));
                
            return result.Build(user.Config.Tooltips);
        }
    }
}
