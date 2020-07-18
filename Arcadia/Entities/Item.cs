using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia
{
    public class Item
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public List<string> Quotes { get; set; }

        public ItemTag Tag { get; set; }

        public ItemRarity Rarity { get; set; }

        public long Value { get; set; }

        // This determines how much space they take up.
        public long Size { get; set; }

        public bool CanSell { get; set; }

        public bool CanBuy { get; set; }

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

        public ItemAction OnUse { get; set; }

        // If this function is ever true, it will expire
        public Func<bool> ToExpire { get; set; }
    }
}
