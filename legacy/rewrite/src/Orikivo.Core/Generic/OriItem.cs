using Newtonsoft.Json;
using Orikivo.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // classes are dynamic objects
    // structs are static objects
    public class OriItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("flavor")]
        public string FlavorText { get; set; }

        [JsonProperty("icon")]
        private string _iconPath { get; set; }

        public string IconPath { get { return $"{Locator.Resources}//icons//{_iconPath}"; } }
        [JsonProperty("group")]
        public OriItemGroupType Group { get; set; }
        [JsonProperty("index")]
        public ushort Index { get; set; }
        [JsonProperty("rarity")]
        public OriItemRarityType Rarity { get; set; }
        
        [JsonProperty("market_tag")]
        public OriItemMarketTag MarketTag { get; set; }
        [JsonProperty("trade_tag")]
        public OriItemTradeTag TradeTag { get; set; }
        [JsonProperty("gift_tag")]
        public OriItemGiftTag GiftTag { get; set; }
        [JsonProperty("action_tag")]
        public OriItemActionTag ActionTag { get; set; }

        public uint Id { get { return uint.Parse($"{((ushort)Group).ToString("00")}{Index.ToString("000")}"); } }
    }
    // pocket lawyer continuously grows in cost when used on your own terms
    public class OriItemMarketTag
    {
        [JsonProperty("criteria")]
        public List<OriItemCriterion> Criteria { get; set; }
        [JsonProperty("value")]
        public ulong? Value { get; set; }
        [JsonProperty("max")]
        public ushort? Limit { get; set; } // the most you can hold at once.
        [JsonProperty("can_buy")]
        public bool IsBuyable { get; set; }
        [JsonProperty("resale")]
        public double? ResaleValue { get; set; } // ~70% by default.
        [JsonProperty("can_sell")]
        public bool IsSellable { get; set; }
    }

    public class OriItemTradeTag
    {
        [JsonProperty("criteria")]
        public List<OriItemCriterion> Criteria { get; set; }
        [JsonProperty("max")]
        public ushort? Limit { get; set; } // amount of times you can trade; otherwise infinite.
        // before it expires.
        [JsonProperty("can_trade")]
        public bool IsTradable { get; set; } // base off of criteria.
    }

    public class OriItemGiftTag
    {
        [JsonProperty("criteria")]
        public List<OriItemCriterion> Criteria { get; set; }
        [JsonProperty("can_gift")]
        public bool IsGiftable { get; set; }
    }

    public class OriItemActionTag
    {
        [JsonProperty("action")]
        public OriItemActionBlock Action { get; set; }
        [JsonProperty("max")]
        public ushort? Limit { get; set; } // number of times you can use; otherwise once.
        [JsonProperty("can_use")]
        public bool IsUsable { get; set; }
        [JsonProperty("value")]
        public ulong value { get; set; } // the amount of n action, if the action is to give a stackable item.
        [JsonProperty("has_cooldown")]
        public bool HasCooldown { get; set; }
        [JsonProperty("duration")]
        public int DurationMinutes { get; set; }
    }

    // if the state of something related to the item upon action
    // is true.
    public class OriItemCriterion
    {
        public Action Ensure { get; set; }
    }

    // an action that an item can use, if any.
    public enum OriItemActionBlock : ushort
    {
        ClearDebt = 1,
        SetColorPacket = 2,
        SetFont = 3,
        GiveMoney = 4
    }

    public enum OriItemGroupType : ushort
    {
        Booster = 1,
        Border = 3,
        Backdrop = 4,
        Font = 5,
        ColorPacket = 6,
        Tool = 8,
        Entity = 16
    }

    public enum OriItemRarityType : ushort
    {
        Abundant = 2,
        Restricted = 3,
        Elite = 4,
        Covert = 5,
        Scarce = 6,
        Singularity = 10
    }
}
