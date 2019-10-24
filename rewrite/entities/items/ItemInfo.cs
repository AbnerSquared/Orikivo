using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents the internal properties of an item.
    /// </summary>
    public class ItemInfo
    {
        // TODO: Generate the ID automatically.
        /// <summary>
        /// A global unique identifier.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// The global name that is used for the item.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Describes what the item is, alongside what it does.
        /// </summary>
        public string Summary { get; internal set; } // this item does this

        /// <summary>
        /// The rarity of the item.
        /// </summary>
        public ItemRarity Rarity {get; internal set; }

        /// <summary>
        /// The group that the item is in, if one is specified. This serves as its generic type.
        /// </summary>
        public ItemGroup? Group { get; internal set; }

        public ItemMarketInfo MarketInfo { get; internal set; }

        public ItemGiftInfo GiftInfo { get; internal set; }

        public ItemDecayInfo DecayInfo { get; internal set; }

        public ItemTradeInfo TradeInfo { get; internal set; }

        public ItemActionInfo ActionInfo { get; internal set; }

        /// <summary>
        /// Defines all of the requirements a user needs to own this item.
        /// </summary>
        public UserCriteria ToOwn { get; internal set; }

        /// <summary>
        /// Defines if the item is unique.
        /// </summary>
        public bool CanStack { get; internal set; }

        public ItemData CreateDataPacket()
        {
            UniqueItemData unique = new UniqueItemData
            {
                ExpiresOn = DecayInfo?.ExpiresOn,
                UsesLeft = ActionInfo?.MaxUses,
                TradesLeft = TradeInfo?.MaxTrades,
                GiftsLeft = GiftInfo?.MaxGifts
            };

            ItemData data = new ItemData(unique);
            return data;
        }
    }
}
