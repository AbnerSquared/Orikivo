using Newtonsoft.Json;
using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    // this is used to store the memory of this catalog.
    public class CatalogData
    {
        [JsonConstructor]
        internal CatalogData(DateTime generatedAt, Dictionary<string, int> itemIds)
        {
            GeneratedAt = generatedAt;
            ItemIds = itemIds;
        }


        [JsonProperty("generated_at")]
        public DateTime GeneratedAt { get; }

        [JsonProperty("item_ids")]
        public Dictionary<string, int> ItemIds { get; }

        [JsonIgnore]
        public int Count => ItemIds.Values.Sum();

        public MarketCatalog Decompress()
        {
            return new MarketCatalog(this);
        }
    }
}
