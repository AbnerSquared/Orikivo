using System;
using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents a generic interactive object.
    /// </summary>
    public class Item
    {
        public static readonly int MaxNameLength = 32;

        public string Id { get; set; }

        public string GroupId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Summary { get; set; }

        public List<string> Quotes { get; set; }

        public ItemTag Tag { get; set; }

        public ItemRarity Rarity { get; set; }

        public CurrencyType Currency { get; set; } = CurrencyType.Money;

        public long Value { get; set; }

        public ItemDeny DeniedHandles { get; set; }

        public long Size { get; set; }

        public bool CanSell => !DeniedHandles.HasFlag(ItemDeny.Sell);

        public bool CanBuy => !DeniedHandles.HasFlag(ItemDeny.Buy);

        public Func<ItemMarketAction, UniqueItemData, float> MarketCriteria { get; set; }

        public int? TradeLimit { get; set; }

        public int? OwnLimit { get; set; }

        public bool BypassCriteriaOnTrade { get; set; }

        public Func<ArcadeUser, bool> ToOwn { get; set; }

        public ItemUsage Usage { get; set; }

        public Func<bool> ToExpire { get; set; }

        public List<ItemProperty> Properties { get; set; }

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
            if (ItemHelper.TryGetGroup(GroupId, out ItemGroup group))
                return group.Summary;

            return Summary;
        }

        public string GetIcon()
        {
            if (ItemHelper.TryGetGroup(GroupId, out ItemGroup group))
                return group.Icon;

            return Icon ?? "";
        }
    }
}
