using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a collection of actions that <see cref="Item"/> can allow.
    /// </summary>
    [Flags]
    public enum ItemAllow
    {
        Buy = 1,
        Sell = 2,
        Clone = 4,
        Seal = 8,
        Delete = 16,
        Rename = 32,
        All = Buy | Sell | Clone | Seal | Delete | Rename
    }
}
