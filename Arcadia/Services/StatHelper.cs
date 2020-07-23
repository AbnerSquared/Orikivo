using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public static class StatHelper
    {
        public static readonly Dictionary<string, string> Names = new Dictionary<string, string>
        {
        };

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

        public static void SetIfLesser(ArcadeUser user, string a, string b)
        {
            if (user.GetStat(b) < user.GetStat(a))
                user.SetStat(a, user.GetStat(b));
        }

        // gets the diff between 2 stats
        public static long Difference(ArcadeUser user, string a, string b)
        {
            return user.GetStat(b) - user.GetStat(a);
        }

        public static long Sum(ArcadeUser user, string a, string b)
            => user.GetStat(a) + user.GetStat(b);

        

        internal static string NameOf(string stat)
        {
            return Names.ContainsKey(stat) ? Format.Bold(Names[stat]) : Format.LineCode(stat);
        }
    }
}