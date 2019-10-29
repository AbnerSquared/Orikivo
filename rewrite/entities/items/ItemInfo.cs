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

        // What to do when marketing, if possible.
        public ItemMarketInfo MarketInfo { get; internal set; }

        // What to do when gifting, if possible.
        public ItemGiftInfo GiftInfo { get; internal set; }

        // What to do on decay, if possible
        public ItemDecayInfo DecayInfo { get; internal set; }

        // What to do on trading, if possible
        public ItemTradeInfo TradeInfo { get; internal set; }


        // What to do on use, if any.
        public ItemActionInfo ActionInfo { get; internal set; }

        /// <summary>
        /// Defines all of the requirements a user needs to own this item.
        /// </summary>
        public AccountCriteria ToOwn { get; internal set; }

        private bool IsUnique()
            => (DecayInfo?.ExpiresOn.HasValue ?? false) &&
            (ActionInfo?.MaxUses.HasValue ?? false) &&
            (TradeInfo?.MaxTrades.HasValue ?? false) &&
            (GiftInfo?.MaxGifts.HasValue ?? false);

        public ItemData CreateDataPacket()
        {
            if (IsUnique())
            {
                UniqueItemData unique = new UniqueItemData
                {
                    ExpiresOn = DecayInfo?.ExpiresOn,
                    UsesLeft = ActionInfo?.MaxUses,
                    TradesLeft = TradeInfo?.MaxTrades,
                    GiftsLeft = GiftInfo?.MaxGifts
                };

                return new ItemData(Id, unique);
            }
            return new ItemData(Id, 1);
        }
    }
}
