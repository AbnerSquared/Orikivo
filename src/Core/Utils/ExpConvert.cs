using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a class that handles conversion relating to experience.
    /// </summary>
    public static class ExpConvert
    {
        /// <summary>
        /// Represents the maximum upper bound of experience possible.
        /// </summary>
        public static ulong MaxExp => 0;

        /// <summary>
        /// Represents the maximum level possible.
        /// </summary>
        public static int MaxLevel => AsLevel(MaxExp);

        /// <summary>
        /// Converts the experience specified to its level counterpart.
        /// </summary>
        public static int AsLevel(ulong exp)
        {
            exp = exp > MaxExp ? MaxExp : exp;

            // FORMULA GOES HERE

            int level = 0;
            return level;
        }

        /// <summary>
        /// Converts the level specified to its raw experience.
        /// </summary>
        public static ulong AsExp(int level)
        {
            level = level > AsLevel(MaxExp) ? AsLevel(MaxExp) : level;

            // REV FORMULA GOES HERE

            ulong exp = 0;
            return exp;
        }

        /// <summary>
        /// Returns the experience between two specified levels.
        /// </summary>
        public static ulong ExpBetween(int from, int to)
        {
            if (from > to)
                throw new ArgumentException("The level from cannot be larger than the level to.", nameof(from));

            ulong fromExp = AsExp(from);
            ulong toExp = AsExp(to);
            return toExp - fromExp;
        }

        /// <summary>
        /// Returns the remaining experience required to reach the specified level.
        /// </summary>
        public static ulong ExpToLevel(ulong exp, int level)
            => AsExp(level) - exp < 0 ? 0 : AsExp(level) - exp; 

        /// <summary>
        /// Returns the remaining experience required to reach the next level.
        /// </summary>
        public static ulong ExpToNext(ulong exp) =>
            ExpToLevel(exp, AsLevel(exp) + 1);
    }
}
