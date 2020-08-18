using Newtonsoft.Json;
using System;

namespace Arcadia
{

    public enum CooldownCategory
    {
        Instance = 1, // Applied to this unique instance only
        Item = 2, // Applied to this item only
        Group = 3, // Applied to this item's group only (throw an exception if no group is specified
        Global = 4 // Applied to all items as a whole
    }

    [Flags]
    public enum ExpiryTrigger
    {
        // Expiration starts when first given
        Own = 1,

        // Expiration starts when first used
        Use = 2,

        // Expiration starts when traded
        Trade = 4,

        // Expiration starts when gifted
        Gift = 8,

        // Expiration starts when equipped
        Equip = 16
    }

    public enum UsageType
    {

        Tool = 1,

        // Durability is ignored; when used, the Expiry is applied and left there.
        Booster = 2,

        // Item can be equipped
        Equipment = 3

    }

    // This specifies how the item is deleted
    [Flags]
    public enum ItemDeleteMode
    {
        Break = 1, // this item is deleted when broken (durability = 0)
        Expire = 2, // this item is deleted when expired (ExpiresOn < DateTime.UtcNow)
        Any = Break | Expire // This item is deleted when broken or expired
    }

    public class ItemEquip
    {
        internal ItemEquip() { }

        // Can this equipment pause the expiration when taken off?
        public bool PauseOnRemove { get; set; }

        // Can this equipment be removed?
        public bool CanRemove { get; set; }
    }

    public class ItemAction
    {
        internal ItemAction() { }

        // The amount of times that this item can be used.
        public int? Durability { get; set; }

        // The cooldown
        public TimeSpan? Cooldown { get; set; }

        public TimeSpan? Expiry { get; set; }

        public ExpiryTrigger ExpiryTrigger { get; set; } = ExpiryTrigger.Own;

        public CooldownCategory CooldownCategory { get; set; } = CooldownCategory.Item;

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

        public ItemDeleteMode DeleteMode { get; set; }
    }
}
