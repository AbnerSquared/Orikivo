using System;

namespace Arcadia
{
    [Flags]
    public enum ItemAllow
    {
        Buy = 1,
        Sell = 2,
        Clone = 4,
        Seal = 8,
        All = Buy | Sell | Clone | Seal
    }
}
