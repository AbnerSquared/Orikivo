using System;

namespace Arcadia
{
    [Flags]
    public enum ShopAllow
    {
        Buy = 1,
        Sell = 2,
        All = Buy | Sell
    }
}