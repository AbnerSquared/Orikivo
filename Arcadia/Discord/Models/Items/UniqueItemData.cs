using Newtonsoft.Json;
using Orikivo;
using System;
using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents unique data for an <see cref="Item"/>.
    /// </summary>
    public class UniqueItemData
    {
        internal UniqueItemData()
        {
            Id = KeyBuilder.Generate(5);
        }

        [JsonConstructor]
        internal UniqueItemData(string id, string name, int? durability, DateTime? expiresOn,
            DateTime? lastUsed, int? tradeCount,
            Dictionary<string, ItemPropertyData> properties)
        {
            Id = id;
            Name = name;
            Durability = durability;
            ExpiresOn = expiresOn;
            LastUsed = lastUsed;
            TradeCount = tradeCount;
            Properties = properties;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("durability")]
        public int? Durability { get; internal set; }

        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }

        [JsonProperty("last_used")]
        public DateTime? LastUsed { get; internal set; }

        [JsonProperty("trade_count")]
        public int? TradeCount { get; internal set; }

        // This is everything that the item is keeping track of
        [JsonProperty("properties")]
        public Dictionary<string, ItemPropertyData> Properties { get; internal set; }
    }
}
