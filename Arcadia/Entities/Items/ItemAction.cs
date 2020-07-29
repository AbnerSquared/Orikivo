using Newtonsoft.Json;
using System;

namespace Arcadia
{

    public enum UsageCooldownType
    {
        Single = 1, // If set to single the cooldown is applied to the item ID, marking any items with the same ID on a cooldown
        Group = 2 // If set to group, the cooldown is applied at a group level, marking any groups using the same group as the specified item
    }

    public class ItemAction
    {
        internal ItemAction() { }

        // The amount of times that this item can be used.
        public int? Durability { get; set; }

        public TimeSpan? Cooldown { get; set; }

        // TODO Implement cooldown grouping
        public UsageCooldownType CooldownType { get; set; } = UsageCooldownType.Single;

        // What happens when this is used?
        // string represents the message sent. If unspecified, it will use the default text on using an item.
        // Action<ArcadeUser, string> Action;
        public Func<ArcadeUser, UsageResult> Action { get; set; }

        public Func<ArcadeUser, bool> Criteria { get; set; }

        // What happens when this item is broken?
        // This can be used to apply debuffs.
        public Action<ArcadeUser> OnBreak { get; set; }

        // Is this item deleted when broken?
        public bool DeleteOnBreak { get; set; }
    }
}
