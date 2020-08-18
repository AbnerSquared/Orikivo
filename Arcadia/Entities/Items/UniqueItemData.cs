using Newtonsoft.Json;
using Orikivo;
using System;
using System.Collections.Generic;

namespace Arcadia
{
    public class UniqueItemData
    {
        internal UniqueItemData()
        {
            Id = KeyBuilder.Generate(5);
        }

        [JsonConstructor]
        internal UniqueItemData(string id, int? durability, DateTime? expiresOn, DateTime? lastUsed, int? tradeCount, int? giftCount, Dictionary<string, int> capsule, Dictionary<string, long> attributes, Dictionary<string, long> toUnlock)
        {
            Id = id;
            Durability = durability;
            ExpiresOn = expiresOn;
            LastUsed = lastUsed;
            TradeCount = tradeCount;
            GiftCount = giftCount;
            Children = capsule;
            Attributes = attributes;
            ToUnlock = toUnlock;
        }

        [JsonProperty("id")]
        public string Id { get; }

        // If true, this item cannot be auto-used, removed, sold, traded, gifted.
        [JsonProperty("locked")]
        public bool Locked { get; internal set; }

        [JsonProperty("durability")]
        public int? Durability { get; internal set; }

        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }

        [JsonProperty("last_used")]
        public DateTime? LastUsed { get; internal set; }

        [JsonProperty("trade_count")]
        public int? TradeCount { get; internal set; }

        [JsonProperty("gift_count")]
        public int? GiftCount { get; internal set; }

        // This is everything that is stored in the item
        [JsonProperty("children")]
        public Dictionary<string, int> Children { get; internal set; }

        // This is everything that the item is keeping track of
        [JsonProperty("attributes")]
        public Dictionary<string, long> Attributes { get; internal set; }

        // A list of stats that need to be at a certain value in order to unlock
        [JsonProperty("to_unlock")]
        public Dictionary<string, long> ToUnlock { get; internal set; }
    }

    public class ItemProperty
    {
        // The ID of this property
        public string Id { get; set; }

        // The current value of this item property
        public long Value { get; set; }

        // When does this property expire?
        public DateTime? ExpiresOn { get; set; }

        // What is the durability counter?
        public int? Durability { get; set; }
    }
}
