using System;
using System.Collections.Generic;
using Arcadia.Models;
using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents a generic interactive object.
    /// </summary>
    public class Item : IModel<string>
    {
        public static readonly int MaxNameLength = 32;

        /// <summary>
        /// Represents the unique identifier for this <see cref="Item"/>.
        /// </summary>
        public string Id { get; set; }

        public int MemoId { get; set; }

        /// <summary>
        /// Represents the group ID for this <see cref="Item"/> (optional).
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Represents the display name for this <see cref="Item"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the display icon for this <see cref="Item"/>.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Represents the summary for this <see cref="Item"/>.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Represents the base research memo for this <see cref="Item"/>.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Represents a collection of display quotes when this <see cref="Item"/> is inspected.
        /// </summary>
        public List<string> Quotes { get; set; }

        /// <summary>
        /// Represents the set of identifier tags for this <see cref="Item"/>.
        /// </summary>
        public ItemTag Tag { get; set; }

        /// <summary>
        /// Represents the relative rarity for this <see cref="Item"/>.
        /// </summary>
        public ItemRarity Rarity { get; set; }

        /// <summary>
        /// Represents the currency type that this <see cref="Item"/> is handled in.
        /// </summary>
        public CurrencyType Currency { get; set; } = CurrencyType.Money;

        /// <summary>
        /// Represents the base value of this <see cref="Item"/>.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Represents the collection of allowed action handles for this <see cref="Item"/>.
        /// </summary>
        public ItemAllow AllowedHandles { get; set; } = ItemAllow.All;

        /// <summary>
        /// Represents the storage size of this <see cref="Item"/>.
        /// </summary>
        public long Size { get; set; }

        // NOTE: This is to check if items can be sold based on the current quality of the item.
        /// <summary>
        /// Determines if an instance of this <see cref="Item"/> can allow the specified economy actions (optional, unimplemented).
        /// </summary>
        public Dictionary<EconomyAction, Func<ItemData, bool>> EconomyCriteria { get; set; }

        /// <summary>
        /// Determines the trade limit for this <see cref="Item"/>.
        /// </summary>
        public int? TradeLimit { get; set; }

        /// <summary>
        /// Represents the maximum allowed amount of this <see cref="Item"/>.
        /// </summary>
        public int? OwnLimit { get; set; }

        /// <summary>
        /// When true, bypasses all criteria required to own this <see cref="Item"/>.
        /// </summary>
        public bool BypassCriteriaOnTrade { get; set; }

        /// <summary>
        /// Represents the method that is invoked when determining if a user can own this <see cref="Item"/>.
        /// </summary>
        public Func<ArcadeUser, bool> ToOwn { get; set; }

        /// <summary>
        /// Represents usage information for this <see cref="Item"/> (optional).
        /// </summary>
        public ItemUsage Usage { get; set; }

        // NOTE: This is only going to be used in cases where an item needs to be removed, since this is a direct way to manage that.
        /// <summary>
        /// Represents the method that is invoked to mark this <see cref="Item"/> as expired.
        /// </summary>
        public Func<bool> ToExpire { get; set; }

        /// <summary>
        /// Represents a collection of custom properties for this <see cref="Item"/>.
        /// </summary>
        public List<ItemProperty> Properties { get; set; }

        // Represents a collection of property IDs that can be specified for this item
        // If unspecified, any property ID can be set.
        // All dynamic properties that are specified in Item.Properties are included
        /// <summary>
        /// Specifies a collection of whitelisted property IDs that can be used with this <see cref="Item"/>. (optional)
        /// </summary>
        public List<string> AllowedPropertyIds { get; set; } = new List<string>();

        /// <summary>
        /// Represents a collection of research tiers for this <see cref="Item"/>.
        /// </summary>
        public Dictionary<int, string> ResearchTiers { get; set; }

        public bool CanSell => AllowedHandles.HasFlag(ItemAllow.Sell);

        public bool CanBuy => AllowedHandles.HasFlag(ItemAllow.Buy);

        public string GetName()
        {
            if (ItemHelper.TryGetGroup(GroupId, out ItemGroup group))
                return $"{group.Prefix}{Name}";

            return Name ?? Id;
        }

        public string GetQuote()
            => Check.NotNullOrEmpty(Quotes) ? Randomizer.Choose(Quotes) : null;

        public string GetSummary()
        {
            if (Check.NotNull(Summary))
                return Summary;

            if (ItemHelper.TryGetGroup(GroupId, out ItemGroup group))
                return group.Summary;

            return Summary;
        }

        public string GetIcon()
        {
            if (Check.NotNull(Icon))
                return Icon;

            if (ItemHelper.TryGetGroup(GroupId, out ItemGroup group))
                return group.Icon?.ToString() ?? "";

            return Icon ?? "";
        }
    }
}
