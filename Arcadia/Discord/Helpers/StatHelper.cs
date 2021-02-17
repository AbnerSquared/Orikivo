using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Services;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public static class StatHelper
    {
        public static DateTime AsTime(ArcadeUser user, string statId)
            => new DateTime(user.GetVar(statId));

        public static TimeSpan GetRemainder(ArcadeUser user, string statId, TimeSpan cooldown)
            => cooldown - SinceLast(user, statId);

        public static TimeSpan SinceLast(ArcadeUser user, string statId)
            => TimeSpan.FromTicks(DateTime.UtcNow.Ticks - user.GetVar(statId));

        public static TimeSpan SinceTime(ArcadeUser user, string statId, DateTime time)
            => TimeSpan.FromTicks(time.Ticks - user.GetVar(statId));

        private static IEnumerable<KeyValuePair<string, long>> GetVisibleStats(ArcadeUser user)
            => user.Stats.Where(x =>
                x.Value != 0
                && Var.TypeOf(x.Key) == VarType.Stat
                && !ItemHelper.Exists(Var.GetGroup(x.Key))
                && !ShopHelper.Exists(Var.GetGroup(x.Key))
                ).OrderBy(x => x.Key);

        private static IEnumerable<KeyValuePair<string, long>> GetGroupStats(ArcadeUser user, string group)
            => user.Stats.Where(x =>
                x.Value != 0
                && Var.GetGroup(x.Key) == group)
                .OrderBy(x => x.Key);

        // 35 / 25 => 1.23

        public static string GetRandomStat(ArcadeUser user, IEnumerable<string> chosen)
            => Randomizer.ChooseOrDefault(user.Stats.Keys.Where(x =>
                user.GetVar(x) != 0
                && !x.EqualsAny(Vars.Balance, Vars.Debt, Vars.Chips, Vars.Tokens)
                && Var.TypeOf(x) == VarType.Stat
                && !ItemHelper.Exists(Var.GetGroup(x))
                && !ShopHelper.Exists(Var.GetGroup(x))
                && (Check.NotNullOrEmpty(chosen) ? !chosen.Contains(x) : true)));

        private static string ViewBase(ArcadeUser user, int page = 0)
        {
            bool showTooltips = user.Config.Tooltips;
            var result = new StringBuilder();

            if (showTooltips)
            {
                List<string> tips = new List<string>
                {
                    "Type `stats all` to view a list of all stats being tracked.",
                    "Type `stats <group>` to view all of the stats in a specific group.",
                    "Type `stats <id>` to view a specific stat."
                };

                result.AppendLine(Format.Tooltip(tips))
                    .AppendLine();
            }

            result.AppendLine("> **Statistics**")
                .AppendLine($"Stats Tracked: {user.Stats.Count}")
                .AppendLine();
            
            foreach ((string group, int count) in GetStatGroupCounts(user))
            {
                result.AppendLine($"> **{group}** (**{count:##,0}** {Format.TryPluralize("entry", "entries", count)}");
            }

            return result.ToString();
        }

        public static string ViewGroup(ArcadeUser user, string group, int page = 0, bool isSelf = true)
        {
            var result = new StringBuilder();

            IEnumerable<KeyValuePair<string, long>> stats = GetGroupStats(user, group);

            if (!stats.Any())
                return Format.Warning("The stat group you specified doesn't exist.");

            VarGroup groupInfo = Var.GetGroupDefiner(group);
            if (groupInfo != null && groupInfo.Writer != null)
            {
                result.AppendLine($"> **Stats: {Var.HumanizeGroup(groupInfo.Id)}**");
                result.AppendLine(groupInfo.Writer?.Invoke(user));
                return result.ToString();
            }

            int pageCount = Paginate.GetPageCount(stats.Count(), 10);
            page = Paginate.ClampIndex(page, pageCount);

            string counter = "";

            if (pageCount > 1)
                counter = $" ({Format.PageCount(page + 1, pageCount)})";

            result.AppendLine($"> **Stats: {Var.HumanizeGroup(group)}**{counter}");

            if (pageCount > 1)
                result.AppendLine();

            foreach ((string id, long value) in Paginate.GroupAt(stats, page, 10))
            {
                string name = Var.HumanizeKey(id);
                string valueText = Var.WriteValue(id, value);
                result.AppendLine($"• {name}: **{valueText}**");
            }

            return result.ToString();
        }

        private static IEnumerable<(string, int)> GetStatGroupCounts(ArcadeUser user)
        {
            var groups = new List<(string, int)>();

            foreach ((string id, long value) in user.Stats)
            {
                if (!Var.IsGroupDefined(Var.GetGroup(id)))
                    continue;

                if (!groups.Any(x => x.Item1 == Var.GetGroup(id)))
                {
                    int count = user.Stats.Count(y => Var.GetGroup(y.Key) == Var.GetGroup(id));
                    if (count > 0)
                        groups.Add((Var.HumanizeGroup(id), count));

                    continue;
                }
            }

            return groups;

            //return user.Stats.Select(x => (Var.HumanizeGroup(x.Key), user.Stats.Count(x => Var.EqualsGroup(x.Key, Var.GetGroup(x.Key)))))
                //.Where(x => x.Item2 > 0);
        }

        public static string Write(ArcadeUser user, bool isSelf = true, int page = 0, string input = null)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (isSelf)
                    return ViewBase(user, page);
                else
                    return ViewAll(user, isSelf, page);
            }
                
            if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ViewAll(user, isSelf, page);

            return ViewGroup(user, input, page, isSelf);
        }

        public static string ViewAll(ArcadeUser user, bool isSelf, int page = 0)
        {
            var result = new StringBuilder();

            List<KeyValuePair<string, long>> stats = GetVisibleStats(user).ToList();
            int pageCount = Paginate.GetPageCount(stats.Count, 20);
            page = Paginate.ClampIndex(page, pageCount);

            string counter = null;

            if (pageCount > 1)
                counter = $" ({Format.PageCount(page + 1, pageCount)})";


            result.AppendLine(Locale.GetHeader(Headers.Stat, counter, group: isSelf ? null : user.Username));
            result.AppendLine();

            foreach ((string id, long value) in Paginate.GroupAt(stats, page, 20))
            {
                string name = Var.HumanizeKey(id);
                string valueText = Var.WriteValue(id, value);
                result.AppendLine($"• {name}: **{valueText}**");
            }

            if (stats.Count == 0)
                result.Append("> There doesn't seem to be any visible stats here.");

            return result.ToString();
        }

        public static void AppendViewIterator<T>(IEnumerable<T> collection, ref StringBuilder content, Action<T, StringBuilder> actionPerElement, string contentIfEmpty = null)
        {
            if (!collection.Any())
            {
                if (string.IsNullOrWhiteSpace(contentIfEmpty))
                    content.Append(contentIfEmpty);

                return;
            }

            foreach (T element in collection)
            {
                actionPerElement.Invoke(element, content);
            }
        }

        public static string ViewDetails(ArcadeUser user, string id, in IEnumerable<ArcadeUser> users = null)
        {
            if (!user.Stats.ContainsKey(id))
                return $"> {Icons.Warning} Unknown stat specified.";

            var details = new StringBuilder();

            string name = Var.Humanize(id);
            string value = Var.WriteValue(user, id);
            string header = string.IsNullOrWhiteSpace(name) ? $"• `{id}`" : $"`{id}`\n• **{name}**";

            details.AppendLine($"{header} = {value}");

            string summary = Var.GetDefiner(id)?.Summary;

            if (!string.IsNullOrWhiteSpace(summary))
                details.AppendLine($"> {summary}");

            VarType type = Var.TypeOf(id);

            if ((users?.Any() ?? false) && type == VarType.Stat)
                details.AppendLine(WriteLeaderboardRank(users, user, id));

            return details.ToString();
        }

        // TODO: Move to a static viewer class
        private static string WriteLeaderboardRank(in IEnumerable<ArcadeUser> users, ArcadeUser user, string id)
            => $"> **Global Leaderboard Rank**: **{LeaderboardViewer.FindPosition(users, user, id):##,0}** out of **{users.Count():##,0}**";
    }
}
