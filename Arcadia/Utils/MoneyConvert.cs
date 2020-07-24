using System;

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

        // Tokens are received from voting
        // Due to their rarity, 1 Token is worth 5 Money

    }
}