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

        public string Id { get; set; }

        public int MemoId { get; set; }

        public string GroupId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Summary { get; set; }

        public string Memo { get; set; }

        public List<string> Quotes { get; set; }

        public Dictionary<int, string> ResearchTiers { get; set; }

        public ItemTag Tag { get; set; }

        public ItemRarity Rarity { get; set; }

        public CurrencyType Currency { get; set; } = CurrencyType.Money;

        public long Value { get; set; }

        public ItemAllow AllowedHandles { get; set; } = ItemAllow.All;

        public long Size { get; set; }

        public bool CanSell => AllowedHandles.HasFlag(ItemAllow.Sell);

        public bool CanBuy => AllowedHandles.HasFlag(ItemAllow.Buy);

        public Func<ItemMarketAction, UniqueItemData, float> MarketCriteria { get; set; }

        public int? TradeLimit { get; set; }

        public int? OwnLimit { get; set; }

        public bool BypassCriteriaOnTrade { get; set; }

        public Func<ArcadeUser, bool> ToOwn { get; set; }

        public ItemUsage Usage { get; set; }

        public Func<bool> ToExpire { get; set; }

        public List<ItemProperty> Properties { get; set; }

        // Represents a collection of property IDs that can be specified for this item
        // If unspecified, any property ID can be set.
        // All dynamic properties that are specified in Item.Properties are included
        public List<string> AllowedPropertyIds { get; set; } = new List<string>();

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
