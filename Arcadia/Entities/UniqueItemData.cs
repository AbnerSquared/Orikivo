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
            Capsule = capsule;
            Attributes = attributes;
            ToUnlock = toUnlock;
        }

        [JsonProperty("id")]
        public string Id { get; }

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
        [JsonProperty("capsule")]
        public Dictionary<string, int> Capsule { get; internal set; }

        // This is everything that the item is keeping track of
        [JsonProperty("attributes")]
        public Dictionary<string, long> Attributes { get; internal set; }
        
        [JsonProperty("to_unlock")]
        public Dictionary<string, long> ToUnlock { get; internal set; }
    }
}
