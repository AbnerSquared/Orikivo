using System;

namespace Orikivo
{
    public static class ExpConvert
    {
        // exp to level
        public static int AsLevel(ulong exp)
        {
            int level = 0;
            // EQ
            if (level > MaxLevel)
                return 0;
            return 0;
        }

        // level to exp
        public static ulong AsExp(int level)
        {

            return 0;
        }

        // exp between two levels
        public static ulong ExpBetween(int from, int to)
        {
            if (from > to)
                throw new ArgumentException("The level from cannot be larger than the level to.", nameof(from));

            ulong fromExp = AsExp(from);
            ulong toExp = AsExp(to);

            return toExp - fromExp;
        }

        // the most exp anyone can obtain
        public static ulong MaxExp = 0;

        // the highest level anyone can reach
        public static int MaxLevel => AsLevel(MaxExp);

        // exp to reach a specified level, starting from specified exp. if it's less than the current exp, return 0.
        public static ulong ExpToLevel(ulong exp, int level) => AsExp(level) - exp < 0 ? 0 : AsExp(level) - exp; 

        // exp to reach the next level
        public static ulong ExpToNext(ulong exp) => ExpToLevel(exp, AsLevel(exp) + 1);
    }
}
