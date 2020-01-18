using System;

namespace Orikivo
{
    /// <summary>
    /// A utility class that handles conversion formulas between experience values.
    /// </summary>
    public static class ExpConvert
    {
        // exp to level
        public static int AsLevel(ulong exp)
        {
            exp = exp > MaxExp ? MaxExp : exp;

            // FORMULA GOES HERE

            int level = 0;


            return level;
        }

        // level to exp
        public static ulong AsExp(int level)
        {
            level = level > AsLevel(MaxExp) ? AsLevel(MaxExp) : level;

            // REV FORMULA GOES HERE

            ulong exp = 0;

            return exp;
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
        //public static int MaxLevel => AsLevel(MaxExp);

        // exp to reach a specified level, starting from specified exp. if it's less than the current exp, return 0.
        public static ulong ExpToLevel(ulong exp, int level) =>
            AsExp(level) - exp < 0 ?
            0 :
            AsExp(level) - exp; 

        // exp to reach the next level
        public static ulong ExpToNext(ulong exp) =>
            ExpToLevel(exp, AsLevel(exp) + 1);
    }
}
