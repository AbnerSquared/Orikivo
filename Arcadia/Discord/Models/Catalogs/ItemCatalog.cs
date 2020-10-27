using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Arcadia
{
    /// <summary>
    /// Represents a collection of generated <see cref="Item"/> entries from a <see cref="CatalogGenerator"/>.
    /// </summary>
    public class ItemCatalog
    {
        internal ItemCatalog(Dictionary<string, int> itemIds, Dictionary<string, int> discounts)
        {
            ItemIds = itemIds?.Count > 0 ? itemIds : throw new Exception("Expected catalog to have at least a single item entry but returned empty");
            Discounts = discounts;
            GeneratedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new <see cref="ItemCatalog"/> that contains properties copied from the specified <see cref="ItemCatalog"/>.
        /// </summary>
        /// <param name="catalog">The <see cref="ItemCatalog"/> to create an instance of.</param>
        public ItemCatalog(ItemCatalog catalog)
        {
            ItemIds = catalog?.ItemIds.Count > 0
                ? new Dictionary<string, int>(catalog.ItemIds)
                : throw new Exception("Expected catalog to have at least a single item entry but returned empty");

            Discounts = catalog.Discounts.Count > 0
                ? new Dictionary<string, int>(catalog.Discounts)
                : new Dictionary<string, int>();

            GeneratedAt = catalog.GeneratedAt;
        }

        [JsonConstructor]
        internal ItemCatalog(DateTime generatedAt, Dictionary<string, int> itemIds, Dictionary<string, int> discounts)
        {
            ItemIds = itemIds;
            Discounts = discounts;
            GeneratedAt = generatedAt;
        }

        /// <summary>
        /// Represents the time at which this <see cref="ItemCatalog"/> was generated.
        /// </summary>
        [JsonProperty("generated_at")]
        public DateTime GeneratedAt { get; }

        /// <summary>
        /// Represents a collection of items that this <see cref="ItemCatalog"/> contains.
        /// </summary>
        [JsonProperty("item_ids")]
        public Dictionary<string, int> ItemIds { get; }

        /// <summary>
        /// Represents a collection of discounts that this <see cref="ItemCatalog"/> offers.
        /// </summary>
        [JsonProperty("discounts")]
        public Dictionary<string, int> Discounts { get; }

        /// <summary>
        /// Represents the total count of items that this <see cref="ItemCatalog"/> contains.
        /// </summary>
        [JsonIgnore]
        public int Count => ItemIds.Values.Sum();
    }
}
