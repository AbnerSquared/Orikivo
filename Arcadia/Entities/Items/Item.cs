using System;
using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public class Item
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string GetName()
        {
            if (ItemHelper.TryGetGroup(GroupId, out ItemGroup group))
                return $"{group.Prefix}{Name}";

            return Name;
        }
        // => ItemHelper.NameOf(Id);

        public string GetQuote()
            => Randomizer.Choose(Quotes);

        public string GetSummary()
        {
            if (Check.NotNull(Summary))
                return Summary;

            if (Check.NotNull(GroupId))
            {
                ItemGroup group = ItemHelper.GetGroup(GroupId);

                if (Check.NotNull(group?.Summary))
                    return group?.Summary;
            }

            return "";
        }

        public string GetIcon()
        {
            if (Check.NotNull(Icon))
                return Icon;

            if (Check.NotNull(GroupId))
            {
                ItemGroup group = ItemHelper.GetGroup(GroupId);

                if (Check.NotNull(group?.Icon))
                    return group?.Icon;
            }

            return "";
        }

        public string Summary { get; set; }

        public List<string> Quotes { get; set; }

        // This is used to set a grouping for a set of items.
        // When searching with this group, it will filter all of the items to the ones that match this group
        public string GroupId { get; set; }

        public ItemTag Tag { get; set; }

        public ItemRarity Rarity { get; set; }

        // Dictionary<CurrencyType, long> Cost
        // CostInfo: Currency, Value, CanBuy, CanSell
        public long Value { get; set; }

        public CurrencyType Currency { get; set; } = CurrencyType.Money;

        // This determines how much space they take up.
        public long Size { get; set; }

        public bool CanSell { get; set; }

        public bool CanBuy { get; set; }

        public bool CanDestroy { get; set; }

        // this represents the criteria needed to be able to
        // buy, sell, trade, or gift the item
        // it returns a multiplier float that will be applied to the
        // base value. If 0, the specified action cannot be performed

        // If null, this will simply refer to the CanSell/CanBuy
        public Func<ItemMarketAction, UniqueItemData, float> MarketCriteria { get; set; }

        public int? TradeLimit { get; set; }

        public int? GiftLimit { get; set; }

        // This is used to limit how much you are allowed to own at a time.
        public int? OwnLimit { get; set; }

        public bool BypassCriteriaOnGift { get; set; }

        // These are the requirements to own this item
        public Func<ArcadeUser, bool> ToOwn { get; set; }

        // These are the requirements to unlock this item
        public Func<ArcadeUser, bool> ToUnlock { get; set; }

        public ItemAction Usage { get; set; }

        // If this function is ever true, it will expire
        public Func<bool> ToExpire { get; set; }

        public Dictionary<string, ItemAttribute> Attributes { get; set; }
    }
}
