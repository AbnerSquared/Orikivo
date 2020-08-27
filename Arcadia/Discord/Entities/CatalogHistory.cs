using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcadia
{
    public class CatalogHistory
    {
        public CatalogHistory()
        {
            PurchasedIds = new Dictionary<string, int>();
            SoldIds = new Dictionary<string, int>();
        }

        [JsonConstructor]
        internal CatalogHistory(Dictionary<string, int> purchasedIds, Dictionary<string, int> soldIds)
        {
            PurchasedIds = purchasedIds ?? new Dictionary<string, int>();
            SoldIds = soldIds ?? new Dictionary<string, int>();
        }

        [JsonProperty("purchased_ids")]
        public Dictionary<string, int> PurchasedIds { get; }

        [JsonProperty("sold_ids")]
        public Dictionary<string, int> SoldIds { get; }

        public void Clear()
        {
            PurchasedIds.Clear();
            SoldIds.Clear();
        }
    }
}