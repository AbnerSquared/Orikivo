using Newtonsoft.Json;
using System;
using Orikivo.Unstable;

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
            TradesLeft = tradesLeft;
            GiftsLeft = giftsLeft;
            UsesLeft = usesLeft;
            ExpiresOn = expiresOn;
            UsedAt = usedAt;
        }

        // TODO: Determine if using a local ID is viable.
        /// <summary>
        /// Represents a local identifier for the <see cref="Item"/> to maintain uniqueness.
        /// </summary>
        [JsonProperty("local_id")]
        public int LocalId { get; internal set; }

        /// <summary>
        /// Gets the amount of trades remaining for the <see cref="Item"/>.
        /// </summary>
        [JsonProperty("trades_left")]
        public int? TradesLeft { get; internal set; }

        /// <summary>
        /// Gets the amount of gifts remaining for the <see cref="Item"/>.
        /// </summary>
        [JsonProperty("gifts_left")]
        public int? GiftsLeft { get; internal set; }

        /// <summary>
        /// Gets the amount of uses remaining for the <see cref="Item"/>.
        /// </summary>
        [JsonProperty("uses_left")]
        public int? UsesLeft { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Item"/> expires.
        /// </summary>
        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Item"/> was last used.
        /// </summary>
        [JsonProperty("used_at")]
        public DateTime? UsedAt { get; internal set; }
    }
}
