﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{
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

        public static void Clear(ArcadeUser user, params string[] stats)
        {
            foreach (string stat in stats)
                user.SetStat(stat, 0);
        }

        // set A to B if B is > than A
        public static void SetIfGreater(ArcadeUser user, string a, string b)
        {
            if (user.GetStat(b) > user.GetStat(a))
                user.SetStat(a, user.GetStat(b));
        }

        public static void SetIfGreater(ArcadeUser user, string a, long b)
        {
            if (b > user.GetStat(a))
                user.SetStat(a, b);
        }

        public static void SetIfEmpty(ArcadeUser user, string stat, long value)
        {
            if (user.GetStat(stat) == 0)
                user.SetStat(stat, value);
        }

        public static void Swap(ArcadeUser user, string a, string b)
        {
            long u = user.GetStat(a);
            user.SetStat(a, user.GetStat(b));
            user.SetStat(b, u);
        }

        public static void SetIfLesser(ArcadeUser user, string a, string b)
        {
            if (user.GetStat(b) < user.GetStat(a))
                user.SetStat(a, user.GetStat(b));
        }

        public static void SetIfLesser(ArcadeUser user, string a, long b)
        {
            if (b < user.GetStat(a))
                user.SetStat(a, b);
        }

        // gets the diff between 2 stats
        public static long Difference(ArcadeUser user, string a, string b)
        {
            return user.GetStat(b) - user.GetStat(a);
        }

        public static long Sum(ArcadeUser user, string a, string b)
            => user.GetStat(a) + user.GetStat(b);

        public static DateTime AsTime(ArcadeUser user, string statId)
            => new DateTime(user.GetStat(statId));

        public static TimeSpan SinceLast(ArcadeUser user, string statId)
            => TimeSpan.FromTicks(DateTime.UtcNow.Ticks - user.GetStat(statId));

        public static TimeSpan SinceTime(ArcadeUser user, string statId, DateTime time)
            => TimeSpan.FromTicks(time.Ticks - user.GetStat(statId));

        public static bool HasName(string id)
            => Descriptions.ContainsKey(id) && Check.NotNull(Descriptions[id]?.Name);

        internal static string NameOf(string statId)
        {
            return HasName(statId) ? Format.Bold(Descriptions[statId].Name) : Format.LineCode(statId);
        }

        private static IEnumerable<KeyValuePair<string, long>> GetVisibleStats(ArcadeUser user)
            => user.Stats.Where((key, value) =>
                !key.StartsWith(StatGroups.Cooldown)
                && value != 0
                && !key.StartsWith(ItemHelper.Items.Select(x => x.Id)));

        private static int GetPageCount(ArcadeUser user, int pageSize)
            => (int) Math.Ceiling(GetVisibleStats(user).Count() / (double) pageSize);
        // 35 / 25 => 1.23
        public static string Write(ArcadeUser user, bool isSelf = true, int page = 0, int pageSize = 25)
        {
            var result = new StringBuilder();
            result.Append("> ⏱️ **Stats");

            if (!isSelf)
                result.Append($": {user.Username}");

            result.AppendLine("**");

            int pageCount = GetPageCount(user, pageSize) - 1;
            var stats = GetVisibleStats(user);

            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            if ((pageCount + 1) > 1)
            {
                result.AppendLine($"> (Page **{page + 1:##,0}**/**{pageCount + 1:##,0}**)");
            }

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