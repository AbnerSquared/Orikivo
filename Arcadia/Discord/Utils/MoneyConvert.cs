using System;
using Orikivo.Drawing;

namespace Arcadia
{
    public static class MoneyConvert
    {
        public const double MoneyToChip = 1 / ChipToMoney;
        public const double ChipToMoney = 0.67;
        public const double TokenToMoney = 5;

        public static long ToChips(long money)
            => (long)Math.Ceiling(money * MoneyToChip);

        public static long ChipsToMoney(long chips)
            => (long)Math.Ceiling(chips * ChipToMoney);

        public static long TokensToMoney(long tokens)
            => (long) Math.Ceiling(tokens * TokenToMoney);

        public static long GetCost(long cost, int discount)
        {
            discount = Math.Clamp(discount, 0, 100);

            if (discount == 0)
                return cost;

            float d = RangeF.Convert(0, 100, 0, 1, discount);
            return (long) MathF.Floor(cost * (1 - d));
        }
    }
}
