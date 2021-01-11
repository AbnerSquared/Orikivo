﻿using System;
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

        public static string WriteFor(ArcadeUser user, string query, int page = 0, int pageSize = 15)
        {
            VarGroup group = Var.GetGroupDefiner(query);

            if (group == null)
                return Format.Warning("Unable to find the specified stat group.");

            var result = new StringBuilder();

            if (group.Writer != null)
            {
                result.AppendLine($"> **Stats: {group.Name}**");
                result.AppendLine(group.Writer.Invoke(user));
                return result.ToString();
            }

            var stats = GetGroupStats(user, group.Id).ToList();
            int pageCount = Paginate.GetPageCount(stats.Count, pageSize);
            page = Paginate.ClampIndex(page, pageCount);

            string counter = null;

            if (pageCount > 1)
                counter = $" ({Format.PageCount(page + 1, pageCount)})";

            result.AppendLine($"> **Stats: {group.Name}**{counter}");

            int i = 0;

            foreach ((string id, long value) in Paginate.GroupAt(stats, page, pageSize))
            {
                if (i >= pageSize)
                    break;

                result.AppendLine($"`{id}`: {value}");
                i++;
            }

            if (i == 0)
                result.AppendLine("An invalid group was specified or an unknown error has occurred.");

            return result.ToString();
        }

        public static string Write(ArcadeUser user, bool isSelf = true, int page = 0, int pageSize = 25)
        {
            var result = new StringBuilder();

            List<KeyValuePair<string, long>> stats = GetVisibleStats(user).ToList();
            int pageCount = Paginate.GetPageCount(stats.Count, pageSize);
            page = Paginate.ClampIndex(page, pageCount);

            string counter = null;

            if (pageCount > 1)
                counter = $" ({Format.PageCount(page + 1, pageCount)})";


            result.AppendLine(Locale.GetHeader(Headers.Stat, counter, group: isSelf ? null : user.Username));

            result.AppendLine();

            int offset = page * pageSize;
            int i = 0;
            foreach ((string id, long value) in stats.Skip(offset))
            {
                if (i >= pageSize)
                    break;

                result.AppendLine($"`{id}`: {value}");
                i++;
            }

            if (i == 0)
                result.Append("> There doesn't seem to be any visible stats here.");

            return result.ToString();
        }

        public static string ViewDetails(ArcadeUser user, string id, in IEnumerable<ArcadeUser> users = null)
        {
            if (!user.Stats.ContainsKey(id))
                return $"> {Icons.Warning} Unknown stat specified.";

            var details = new StringBuilder();

            string name = Var.WriteName(id);
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
            => $"> **Global Leaderboard Rank**: **{Leaderboard.GetPosition(users, user, id):##,0}** out of **{users.Count():##,0}**";
    }
}
