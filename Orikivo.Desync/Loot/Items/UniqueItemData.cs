using Newtonsoft.Json;
using System;
using Orikivo.Desync;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents unique data for an <see cref="Item"/>.
    /// </summary>
    public class UniqueItemData
    {
        internal UniqueItemData() { }

        [JsonConstructor]
        internal UniqueItemData(int localId, int? tradesLeft, int? giftsLeft, int? usesLeft, DateTime? expiresOn, DateTime? usedAt)
        {
            LocalId = localId;
            TradesUsed = tradesLeft;
            GiftsUsed = giftsLeft;
            Durability = usesLeft;
            ExpiresOn = expiresOn;
            LastUsed = usedAt;
        }

        // TODO: Determine if using a local ID is viable.
        /// <summary>
        /// Represents a local identifier for this <see cref="Item"/>.
        /// </summary>
        [JsonProperty("local_id")]
        public int LocalId { get; internal set; }

        /// <summary>
        /// Gets the amount of trades that were executed on this <see cref="Item"/>.
        /// </summary>
        [JsonProperty("trades_used")]
        public int? TradesUsed { get; internal set; }

        /// <summary>
        /// Gets the amount of times this <see cref="Item"/> was gifted.
        /// </summary>
        [JsonProperty("gifts_used")]
        public int? GiftsUsed { get; internal set; }

        /// <summary>
        /// Gets the current remaining durability for this <see cref="Item"/>.
        /// </summary>
        [JsonProperty("durability")]
        public int? Durability { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Item"/> expires.
        /// </summary>
        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Item"/> was last used.
        /// </summary>
        [JsonProperty("last_used")]
        public DateTime? LastUsed { get; internal set; }

        /// <summary>
        /// Gets the information that defines what this <see cref="Item"/> is storing.
        /// </summary>
        [JsonProperty("capsule")]
        public CapsuleData Capsule { get; internal set; }

        /// <summary>
        /// Gets the seed that this <see cref="Item"/> is bound to.
        /// </summary>
        [JsonProperty("seed")]
        public string Seed { get; internal set; }

        /// <summary>
        /// Gets the collection of unique attributes that this <see cref="Item"/> stores.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<string, int> Attributes { get; internal set; }
    }

    /// <summary>
    /// Represents information about what an <see cref="Item"/> is currently containing.
    /// </summary>
    public class CapsuleData
    {
        [JsonConstructor]
        internal CapsuleData(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }

        [JsonProperty("item_id")]
        public string ItemId { get; internal set; }

        [JsonProperty("amount")]
        public int Amount { get; internal set; }
    }
}
