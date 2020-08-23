using System;

namespace Arcadia
{
    [Flags]
    public enum CurrencyType
    {
        Money = 1,
        Chips = 2,
        Tokens = 4,
        Debt = 8
    }
}