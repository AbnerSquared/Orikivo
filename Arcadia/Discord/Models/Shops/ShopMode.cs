using System;

namespace Arcadia
{
    [Flags]
    public enum AllowedShopActions
    {
        Buy = 1,
        Sell = 2,
        Any = Buy | Sell
    }
}