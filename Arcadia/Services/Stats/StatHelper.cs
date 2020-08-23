﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public enum StatQuery
    {
        Default = 1
    }

    public static class StatHelper
    {
        public static readonly Dictionary<string, Descriptor> Descriptions = new Dictionary<string, Descriptor>
        {
            [TickStats.CurrentLossStreak] = new Descriptor
            {
                Summary = "Increases the chance of winning by 1% for every 3 losses in **Doubler**."
            }
        };

        public static string SummaryOf(string id)
        {
            if (Descriptions.ContainsKey(id))
            {
                if (Check.NotNull(Descriptions[id]?.Summary))
                {
                    return Descriptions[id].Summary;
                }
            }

            return null;
        }


        public static long Sum(ArcadeUser user, string a, string b)
            => user.GetVar(a) + user.GetVar(b);

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
                ).OrderBy(x => x.Key);

        private static IEnumerable<KeyValuePair<string, long>> GetGroupStats(ArcadeUser user, string group)
            => user.Stats.Where(x =>
                x.Value != 0
                && Var.GetGroup(x.Key) == group)
                .OrderBy(x => x.Key);

        private static int GetPageCount(ArcadeUser user, int pageSize)
            => (int) Math.Ceiling(GetVisibleStats(user).Count() / (double) pageSize);
        // 35 / 25 => 1.23

        public static string WriteFor(ArcadeUser user, string query, int page = 0, int pageSize = 25)
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
            int pageCount = (int)Math.Ceiling(stats.Count / (double)pageSize) - 1;
            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            string counter = null;

            if (pageCount + 1 > 1)
            {
                if (pageCount + 1 > 1)
                    counter = $" (Page **{page + 1:##,0}**/**{pageCount + 1:##,0}**)";
            }

            result.AppendLine($"> **Stats: {group.Name}**{counter}");

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
                result.AppendLine("An invalid group was specified or an unknown error has occurred.");

            return result.ToString();
        }

        public static string Write(ArcadeUser user, bool isSelf = true, int page = 0, int pageSize = 25)
        {
            var result = new StringBuilder();

            int pageCount = GetPageCount(user, pageSize) - 1;
            IEnumerable<KeyValuePair<string, long>> stats = GetVisibleStats(user);

            page = page < 0 ? 0 : page > pageCount ? pageCount : page;
            string counter = null;
            if (pageCount + 1 > 1)
            {
                if (pageCount + 1 > 1)
                    counter = $"(Page **{page + 1:##,0}**/**{pageCount + 1:##,0}**)";
            }


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
    }
}
