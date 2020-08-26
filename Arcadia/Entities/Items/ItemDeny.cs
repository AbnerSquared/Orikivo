using System;

namespace Arcadia
{
    [Flags]
    public enum ItemDeny
    {
        Buy = 1,
        Sell = 2,
        Clone = 4,
        Seal = 8
    }
}
