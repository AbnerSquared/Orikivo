using System;

namespace Arcadia
{
    public static class ExpConvert
    {
        public static ulong BaseMaxExp => AsExp(MaxLevel, 0);

        public static readonly int MaxLevel = 100;
        public static readonly uint LevelStartScale = 100;
        public static readonly uint LevelTierScale = 1000;
        public static readonly float RatePerAscent = 0.15f;
        public static readonly float BaseAscentRate = 1;
        public static readonly float MaxAscentRate = 2.5f;

        private static ulong GetAscentExp(int ascent)
        {
            return (ulong) Math.Floor(10 * LevelTierScale * Math.Min(BaseAscentRate + (RatePerAscent * ascent), MaxAscentRate));
        }

        private static ulong GetBaseExp()
        {
            ulong sum = 0;

            for (uint l = 1; l < 10; l++)
                sum += l * LevelStartScale;

            return sum;
        }

        private static int GetInverseTier(ulong exp)
        {
            if (exp == 0)
                return 0;

            ulong expSum = GetBaseExp();
            int level = 0;

            if (exp < expSum)
            {
                expSum = 0;
                while (exp > expSum)
                {
                    expSum += (ulong)level * LevelStartScale;

                    if (exp >= expSum)
                        level++;
                }

                return level;
            }

            level = 9;

            ulong tier = 1;
            ulong partial = tier * LevelTierScale * 10;

            while (exp > expSum + partial)
            {
                expSum += partial;
                level += 10;
                tier++;
                partial = tier * LevelTierScale * 10;
            }

            while (exp > expSum)
            {
                expSum += (ulong)Math.Floor((level + 1) / (double)10) * LevelTierScale;

                if (exp >= expSum)
                    level++;
            }

            return level;
        }

        private static ulong GetExpTier(int level)
        {
            if (level <= 0)
                return 0;

            ulong sum = 0;
            int b = Math.Min(level, 9);

            for (uint l = 1; l <= b; l++)
                sum += l * LevelStartScale;

            if (level < 10)
                return sum;

            int tier = (int) Math.Min(Math.Floor((level + 1) / (double)10), 10);

            int leftover = level - 9;
 
            if (leftover >= 10)
            {
                for (int t = 1; t < tier; t++)
                {
                    sum += (ulong)t * LevelTierScale * 10;
                    leftover -= 10;
                }
            }

            // If there were remainder exp values
            if (leftover > 0)
                sum += (ulong) (tier * LevelTierScale * leftover);

            return sum;
        }

        public static ulong GetMaxExp(int ascent)
        {
            return AsExp(MaxLevel, ascent);
        }

        public static int AsLevel(ulong exp, int ascent)
        {
            if (ascent > 0)
                return (int) Math.Floor(exp / (double)GetAscentExp(ascent));

            return GetInverseTier(exp);
        }

        public static ulong AsExp(int level, int ascent)
        {
            if (ascent > 0)
                return (ulong)level * GetAscentExp(ascent);

            return GetExpTier(level);
        }

        public static ulong ExpBetween(int fromLevel, int toLevel, int ascent)
        {
            if (fromLevel > toLevel)
                throw new ArgumentException("The level from cannot be larger than the level to.", nameof(fromLevel));

            return AsExp(toLevel, ascent) - AsExp(fromLevel, ascent);
        }

        public static ulong ExpToLevel(ulong exp, int level, int ascent)
        {
            if (level > MaxLevel)
                level = MaxLevel;

            if (exp >= AsExp(level, ascent))
                return 0;

            return AsExp(level, ascent) - exp;
        }

        public static ulong ExpToNext(ulong exp, int ascent)
            => ExpToLevel(exp, AsLevel(exp, ascent) + 1, ascent);
    }
}
