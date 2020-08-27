using System;

namespace Arcadia
{
    public static class BoostConvert
    {
        public const int MinBoost = 0;
        public const int MaxBoost = 5;
        public const int MaxTakeBoost = 2;
        private const float FLAT_TOLERANCE = 0.001f;

        private static float GetUpperBound(bool negate)
        {
            return negate ? MaxTakeBoost : MaxBoost;
        }

        private static float Clamp(float rate, bool negate)
        {
            float max = GetUpperBound(negate);
            return rate < MinBoost ? MinBoost : rate > max ? max : rate;
        }

        public static float GetRate(float rate, bool negate = false)
        {
            rate = Clamp(rate, negate);
            int n = negate ? 1 : 0;
            int m = negate ? -1 : 1;

            return n + (n + rate * m);
        }

        public static long GetValue(long amount, float rate)
        {
            if (amount == 0)
                return amount;

            bool negate = Math.Sign(amount) != 1;
            return GetValue(Math.Abs(amount), rate, negate);
        }

        public static long GetValue(long amount, float rate, bool negate)
        {
            if (amount == 0)
                return amount;

            if (amount < 0)
                throw new Exception("Cannot negate a value if the negation is explicitly specified");

            float r = GetRate(rate, negate);
            long result = (long)Math.Round(amount * r, MidpointRounding.AwayFromZero);

            if (result == 0 && Math.Abs(r) >= FLAT_TOLERANCE)
                result = 1;

            return result;
        }
    }
}
