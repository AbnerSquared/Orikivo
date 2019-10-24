using Newtonsoft.Json;
using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a unique item data object for items that can vary from each rendition.
    /// </summary>
    public class UniqueItemData
    {
        /// <summary>
        /// The item's local identifier in order to prevent incorrect item storing.
        /// </summary>
        [JsonProperty("local_id")]
        public int LocalId { get; internal set; }

        /// <summary>
        /// The amount of trades remaining before the item becomes untradable.
        /// </summary>
        [JsonProperty("trades_left")]
        public int? TradesLeft { get; internal set; }

        /// <summary>
        /// The amount of gift trades remaining before the item becomes ungiftable.
        /// </summary>
        [JsonProperty("gifts_left")]
        public int? GiftsLeft { get; internal set; }

        /// <summary>
        /// Defines how many uses that the item has left.
        /// </summary>
        [JsonProperty("uses_left")]
        public int? UsesLeft { get; internal set; }

        /// <summary>
        /// Defines when the item will expire on.
        /// </summary>
        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }
    }
}
