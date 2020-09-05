using System;

namespace Arcadia
{
    [Flags]
    public enum ShopMode
    {
        Buy = 1,
        Sell = 2,
        Any = Buy | Sell
    }
}