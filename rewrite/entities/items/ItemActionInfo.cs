using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Defines what an item can do on use.
    /// </summary>
    public class ItemActionInfo
    {
        /// <summary>
        /// The criteria that a user needs to meet in order to use the item. This is mainly used to allow users to own high-tier items, but be limited on what they can do with it.
        /// </summary>
        public AccountCriteria ToUse { get; internal set; }

        /// <summary>
        /// The amount of times an item can be used before it breaks. If left empty, the item will never break.
        /// </summary>
        public int? MaxUses { get; internal set; }

        /// <summary>
        /// The duration of the cooldown (in seconds) set for an item when used. If left empty, no cooldown is set.
        /// </summary>
        public double? CooldownLength { get; internal set; }

        /// <summary>
        /// An update packet that is executed when the item is used.
        /// </summary>
        public AccountUpdatePacket OnUse { get; internal set; }

        // TODO: Make another update packet that modifies stuff when the last use of an item is called.

        /// <summary>
        /// Determines if the item should be automatically removed when the item runs out of uses.
        /// </summary>
        public bool BreakOnLastUse { get; internal set; }
    }
}
