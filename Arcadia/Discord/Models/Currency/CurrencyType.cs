using System;

namespace Arcadia
{
    [Flags]
    public enum CurrencyType
    {
        Cash = 1,
        Token = 2,
        Favor = 4,
        Debt = 8
    }
}
