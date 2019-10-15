using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents an update packet for a user.
    /// </summary>
    public class UserUpdatePacket
    {

    }

    public class ItemActionResult
    {

    }

    public class ItemActionInfo
    {
        // before use
        public UserCriteria ToUse { get; internal set; }

        // generic info
        public ItemCustomAction? OnUse { get; internal set;  } // a custom action that occurs on use
        public int? MaxUses { get; internal set; } // the amount of times it can be used, otherwise infinite
        public double? CooldownLength { get; internal set; }

        // on use
        // negative number removes from the stat specified, if it exists
        public List<(string, int)> UpgradesOnUse { get; internal set; } // upgrades changed on use
        public List<StatUpdateInfo> StatsOnUse { get; internal set; } // stats changed on use
        public List<(string, int)> ItemsOnUse { get; internal set; } // items changed on use
        public ulong? ExpOnUse { get; internal set; } // exp given on use
        public bool BreakOnUse { get; internal set; } // if the item is used up completely, remove it
    }
}
