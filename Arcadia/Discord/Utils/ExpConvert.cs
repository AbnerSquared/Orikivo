using System;

namespace Arcadia
{
    public static class ExpConvert
    {
        public static long BaseMaxExp => AsExp(MaxLevel, 0);

        public static readonly int MaxLevel = 100;
        public static readonly int LevelNumberBase = 10; // Base-10
        public static readonly int LevelStartScale = 100;
        public static readonly int LevelTierScale = 1000;
        public static readonly float RatePerAscent = 0.15f;
        public static readonly float BaseAscentRate = 1;
        public static readonly float MaxAscentRate = 2.5f;

        private static long GetAscentExp(long ascent)
        {
            return (long) Math.Floor(10 * LevelTierScale * Math.Min(BaseAscentRate + (RatePerAscent * ascent), MaxAscentRate));
        }

        private static long GetBaseExp()
        {
            long sum = 0;

            for (int l = 1; l < LevelNumberBase; l++)
                sum += l * LevelStartScale;

            return sum;
        }

        private static int GetInverseTier(long exp)
        {
            if (exp == 0)
                return 0;

            long expSum = GetBaseExp();
            int level = 0;

            if (exp < expSum)
            {
                expSum = 0;
                while (exp > expSum)
                {
                    expSum += (long) (level + 1) * LevelStartScale;

                    if (exp >= expSum)
                        level++;
                }

                return level;
            }

            level = 9;

            long tier = 1;
            long partial = tier * LevelTierScale * LevelNumberBase;

            while (exp > expSum + partial)
            {
                expSum += partial;
                level += LevelNumberBase;
                tier++;
                partial = tier * LevelTierScale * LevelNumberBase;
            }

            while (exp > expSum)
            {
                expSum += (long) Math.Floor((level + 1) / (double) LevelNumberBase) * LevelTierScale;

                if (exp >= expSum)
                    level++;
            }

            return level;
        }

        private static long GetExpTier(int level)
        {
            if (level <= 0)
                return 0;

            long sum = 0;
            int b = Math.Min(level, LevelNumberBase - 1);

            for (int l = 1; l <= b; l++)
                sum += l * LevelStartScale;

            if (level < LevelNumberBase)
                return sum;

            int tier = (int) Math.Min(Math.Floor((level + 1) / (double) LevelNumberBase), LevelNumberBase);

            int leftover = level - (LevelNumberBase - 1);
 
            if (leftover >= LevelNumberBase)
            {
                for (int t = 1; t < tier; t++)
                {
                    sum += (long) t * LevelTierScale * LevelNumberBase;
                    leftover -= LevelNumberBase;
                }
            }

            // If there were remainder exp values
            if (leftover > 0)
                sum += tier * LevelTierScale * leftover;

            return sum;
        }

        public static long GetMaxExp(long ascent)
        {
            return AsExp(MaxLevel, ascent);
        }

        public static int AsLevel(long exp, long ascent)
        {
            if (ascent > 0)
                return (int) Math.Floor(exp / (double) GetAscentExp(ascent));

            return GetInverseTier(exp);
        }

        public static long AsExp(int level, long ascent)
        {
            if (ascent > 0)
                return level * GetAscentExp(ascent);

            return GetExpTier(level);
        }

        public static long ExpBetween(int fromLevel, int toLevel, long ascent)
        {
            if (fromLevel > toLevel)
                throw new ArgumentException("The level from cannot be larger than the level to.", nameof(fromLevel));

            return AsExp(toLevel, ascent) - AsExp(fromLevel, ascent);
        }

        public static long ExpToLevel(long exp, int level, long ascent)
        {
            if (level > MaxLevel)
                level = MaxLevel;

            if (exp >= AsExp(level, ascent))
                return 0;

            return AsExp(level, ascent) - exp;
        }

        public static long ExpToNext(long exp, long ascent)
            => ExpToLevel(exp, AsLevel(exp, ascent) + 1, ascent);
    }
}
