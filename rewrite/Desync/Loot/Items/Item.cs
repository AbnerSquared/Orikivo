using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Item
    {
        public string Id { get; set; }
        public string IconPath { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        public List<string> Quotes { get; set; } = null;

        public ItemTag Tag { get; set; } = 0;

        public ItemRarity Rarity { get; set; } = ItemRarity.Common;

        public ulong Value { get; set; } = 0;

        public bool CanSell { get; set; } = true;

        public bool CanBuy { get; set; } = true;

        public int? TradeLimit { get; set; } = null;

        public int? GiftLimit { get; set; } = null;

        public bool BypassCriteriaOnGift { get; set; } = false;

        public ItemAction Action { get; set; } = null;

        public Func<User, bool> ToUnlock { get; set; } = null;

        public Func<User, bool> ToOwn { get; set; } = null;

        // for the item to no longer be used.
        public Func<bool> ToExpire { get; set; } = null;

        /*
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
         
         */
    }
}
