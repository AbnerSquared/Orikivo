using Orikivo.Drawing;
using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    // remove certain properties
    // make a bare-bones version of this class
    // that can be inherited for Orikivo and Arcadia
    /// <summary>
    /// Represents an interactive object for a <see cref="World"/> or <see cref="User"/>.
    /// </summary>
    public class Item
    {
        public string Id { get; set; }

        /// <summary>
        /// Represents the image that is used when inspecting the <see cref="Item"/>. If none is specified, the image is hidden.
        /// </summary>
        public Sprite Icon { get; set; }
        
        public string Name { get; set; }

        /// <summary>
        /// Represents a quick description of what the <see cref="Item"/> does.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Represents a collection of unique quotes. If none are specified, this field is ignored.
        /// </summary>
        public List<string> Quotes { get; set; } = null;

        /// <summary>
        /// Represents a collection of groups that the <see cref="Item"/> represents.
        /// </summary>
        public ItemType Tag { get; set; } = 0;

        /// <summary>
        /// Represents the dimension that the <see cref="Item"/> is designed for.
        /// </summary>
        public ItemDimension Dimension { get; set; }

        /// <summary>
        /// Represents the rarity of the <see cref="Item"/>.
        /// </summary>
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;

        /// <summary>
        /// Represents the value of the <see cref="Item"/>.
        /// </summary>
        public ulong Value { get; set; } = 0;

        /// <summary>
        /// Represents the maximum stack that this <see cref="Item"/> can go to.
        /// </summary>
        public int StackSize { get; set; } = 1;

        /// <summary>
        /// Determines if the <see cref="Item"/> can be sold.
        /// </summary>
        public bool CanSell { get; set; } = true;

        /// <summary>
        /// Determines if the <see cref="Item"/> can be bought.
        /// </summary>
        public bool CanBuy { get; set; } = true;

        /// <summary>
        /// Gets or sets the total amount of times the <see cref="Item"/> can be traded.
        /// </summary>
        public int? TradeLimit { get; set; } = null;

        /// <summary>
        /// Gets or sets the total amount of times the <see cref="Item"/> can be gifted.
        /// </summary>
        public int? GiftLimit { get; set; } = null;

        /// <summary>
        /// Determines if <see cref="ToOwn"/> is bypassed when the <see cref="Item"/> is sent as a gift.
        /// </summary>
        public bool BypassCriteriaOnGift { get; set; } = false;

        /// <summary>
        /// Represents the action that is performed whenever an <see cref="Item"/> is used. If none is set, the <see cref="Item"/> is unusable.
        /// </summary>
        public ItemAction Action { get; set; } = null;

        /// <summary>
        /// Gets or sets the criteria required to be able to unlock the <see cref="Item"/>.
        /// </summary>
        public Func<Husk, HuskBrain, bool> ToUnlock { get; set; } = null;

        /// <summary>
        /// Gets or sets the criteria required to be able to own the <see cref="Item"/>.
        /// </summary>
        public Func<Husk, HuskBrain, bool> ToOwn { get; set; } = null;

        /// <summary>
        /// Gets or sets the criteria required for the <see cref="Item"/> to expire.
        /// </summary>
        public Func<bool> ToExpire { get; set; } = null;
    }
}
