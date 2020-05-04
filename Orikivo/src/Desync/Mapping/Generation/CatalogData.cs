using Newtonsoft.Json;
using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a compressed <see cref="Catalog"/>.
    /// </summary>
    public class CatalogData
    {
        [JsonConstructor]
        internal CatalogData(DateTime generatedAt, Dictionary<string, int> itemIds, Dictionary<string, float> discounts)
        {
            GeneratedAt = generatedAt;
            ItemIds = itemIds;
            Discounts = discounts ?? new Dictionary<string, float>();
        }

        /// <summary>
        /// Represents the <see cref="DateTime"/> at which the <see cref="Catalog"/> was generated.
        /// </summary>
        [JsonProperty("generated_at")]
        public DateTime GeneratedAt { get; }

        [JsonProperty("item_ids")]
        public Dictionary<string, int> ItemIds { get; }

        [JsonProperty("discounts")]
        public Dictionary<string, float> Discounts { get; }

        [JsonIgnore]
        public int Count => ItemIds.Values.Sum();

        /// <summary>
        /// Returns a <see cref="Catalog"/> from the <see cref="CatalogData"/>'s decompressed values.
        /// </summary>
        public Catalog Decompress()
        {
            return new Catalog(this);
        }
    }
}
