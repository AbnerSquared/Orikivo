using System;

namespace Arcadia
{
    public static class ExpConvert
    {
        public static readonly ulong MaxExp = 0;

        public static int MaxLevel => AsLevel(MaxExp);

        public static int AsLevel(ulong exp)
        {
            exp = exp > MaxExp ? MaxExp : exp;

            // FORMULA

            int level = 0;
            return level;
        }

        public static ulong AsExp(int level)
        {
            level = level > AsLevel(MaxExp) ? AsLevel(MaxExp) : level;

            // INVERSE FORMULA

            ulong exp = 0;
            return exp;
        }

        public static ulong ExpBetween(int fromLevel, int toLevel)
        {
            if (fromLevel > toLevel)
                throw new ArgumentException("The level from cannot be larger than the level to.", nameof(fromLevel));

            return AsExp(toLevel) - AsExp(fromLevel);
        }

        public static ulong ExpToLevel(ulong exp, int level)
        {
            if (exp >= AsExp(level))
                return 0;

            return AsExp(level) - exp;
        }

        public static ulong ExpToNext(ulong exp)
            => ExpToLevel(exp, AsLevel(exp) + 1);
    }
}
