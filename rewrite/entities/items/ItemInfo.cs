using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents the internal properties of an item.
    /// </summary>
    public class ItemInfo
    {
        public string Id { get; internal set; } // generate the id automatically
        public string Name { get; internal set; } // item name
        public string Summary { get; internal set; } // this item does this
        public ItemRarity Rarity {get; internal set; }
        public ItemGroupType? Group { get; internal set; } // the generic type of item
        public ItemMarketInfo MarketInfo { get; internal set; }
        public ItemGiftInfo GiftInfo { get; internal set; }
        public ItemDecayInfo DecayInfo { get; internal set; }
        public ItemTradeInfo TradeInfo { get; internal set; }
        public ItemActionInfo ActionInfo { get; internal set; }

        /// <summary>
        /// Defines all of the requirements needed to own the specified item.
        /// </summary>
        public UserCriteria ToOwn { get; internal set; }
        public bool CanStack { get; internal set; } // determines if StackCount or Info...

        public UniqueItemData GetEntityData()
        {
            UniqueItemData info = new UniqueItemData
            {
                ExpiresOn = DecayInfo?.ExpiresOn,
                UsesLeft = ActionInfo?.MaxUses,
                TradesLeft = TradeInfo?.MaxTrades ?? 0,
                GiftsLeft = GiftInfo?.MaxGifts ?? 0
            };
            return info;
        }
    }
}
