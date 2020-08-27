using System;
using Orikivo.Drawing;

namespace Arcadia
{
    public static class MoneyConvert
    {
        public const double MoneyToChip = 1 / ChipToMoney;
        public const double ChipToMoney = 0.67;
        public const double TokenToMoney = 5;

        public static long GetChips(long money)
            => (long)Math.Ceiling(money * MoneyToChip);

        public static long GetChipMoney(long chips)
            => (long)Math.Ceiling(chips * ChipToMoney);

        public static long GetTokenMoney(long tokens)
            => (long) Math.Ceiling(tokens * TokenToMoney);

        private static int ClampDiscount(int discount)
        {
            return discount < 0 ? 0 : discount > 100 ? 100 : discount;
        }

        public static long GetCost(long cost, int discount)
        {
            discount = ClampDiscount(discount);

            if (discount == 0)
                return cost;

            float d = RangeF.Convert(0, 100, 0, 1, discount);
            return (long) MathF.Floor(cost * (1 - d));
        }

        // Tokens are received from voting
        // Due to their rarity, 1 Token is worth 5 Money

    }
}