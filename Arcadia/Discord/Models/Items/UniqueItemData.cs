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
            DateTime? lastUsed, int? tradeCount, EquipTarget? equipSlot,
            Dictionary<string, ItemPropertyData> properties)
        {
            Id = id;
            Name = name;
            Durability = durability;
            ExpiresOn = expiresOn;
            LastUsed = lastUsed;
            TradeCount = tradeCount;
            EquipSlot = equipSlot;
            Properties = properties;
        }

        /// <summary>
        /// Represents the unique identifier for this <see cref="UniqueItemData"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; }

        /// <summary>
        /// Represents the custom name of this <see cref="UniqueItemData"/>.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Represents the current health pool for this <see cref="UniqueItemData"/>.
        /// </summary>
        [JsonProperty("durability")]
        public int? Durability { get; internal set; }

        // TODO: Remove this property
        /// <summary>
        /// Represents the time that this <see cref="UniqueItemData"/> expires.
        /// </summary>
        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }

        /// <summary>
        /// Represents the time that this <see cref="UniqueItemData"/> was last used.
        /// </summary>
        [JsonProperty("last_used")]
        public DateTime? LastUsed { get; internal set; }

        /// <summary>
        /// Represents the amount of times that this <see cref="UniqueItemData"/> was traded.
        /// </summary>
        [JsonProperty("trade_count")]
        public int? TradeCount { get; internal set; }

        /// <summary>
        /// Represents the slot that this <see cref="UniqueItemData"/> is equipped in (optional).
        /// </summary>
        [JsonProperty("equip_slot")]
        public EquipTarget? EquipSlot { get; internal set; }

        // This is everything that the item is keeping track of
        /// <summary>
        /// Represents a collection of custom properties for this <see cref="UniqueItemData"/>.
        /// </summary>
        [JsonProperty("properties")]
        public Dictionary<string, ItemPropertyData> Properties { get; internal set; }
    }
}
