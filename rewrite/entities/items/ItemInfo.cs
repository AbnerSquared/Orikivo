using System.Text;

namespace Orikivo
{
    // separate hardformat info and dynamic info.
    public class ItemInfo
    {
        public string Id { get; internal set; } // generate the id automatically
        public string Name { get; internal set; } // item name
        public string Summary { get; internal set; } // this item does this
        public ItemRarityType Rarity {get; internal set; }
        public ItemGroupType? Group { get; internal set; } // the generic type of item
        public ItemMarketInfo MarketInfo { get; internal set; }
        public ItemGiftInfo GiftInfo { get; internal set; }
        public ItemDecayInfo DecayInfo { get; internal set; }
        public ItemTradeInfo TradeInfo { get; internal set; }
        public ItemActionInfo ActionInfo { get; internal set; }
        public ItemCriteria ToOwn { get; internal set; } // requirements to own the item
        public bool CanStack { get; internal set; } // determines if StackCount or Info...

        public UniqueItemInfo GetEntityData()
        {
            UniqueItemInfo info = new UniqueItemInfo();
            if (DecayInfo != null)
                info.ExpiresOn = DecayInfo.ExpiresOn;
            if (ActionInfo != null)
                info.UsesLeft = ActionInfo.MaxUses;
            if (TradeInfo != null)
                info.TradesLeft = TradeInfo.MaxTrades;
            else
                info.TradesLeft = 0;
            if (GiftInfo != null)
                info.GiftsLeft = GiftInfo.MaxGifts;
            else
                info.GiftsLeft = 0;
            return info;
        }
    }
}
