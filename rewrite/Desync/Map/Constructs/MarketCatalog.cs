using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    public class MarketCatalog
    {
        public MarketCatalog(Dictionary<Item, int> items)
        {
            Items = items;
            GeneratedAt = DateTime.UtcNow;
        }

        internal MarketCatalog(CatalogData data)
        {
            GeneratedAt = data.GeneratedAt;
            Items = data.ItemIds.ToDictionary(x => WorldEngine.GetItem(x.Key), y => y.Value);
        }

        public DateTime GeneratedAt { get; set; }
        public Dictionary<Item, int> Items { get; }

        public int Count => Items.Values.Sum();

        public CatalogData Compress()
        {
            return new CatalogData(GeneratedAt, Items.ToDictionary(x => x.Key.Id, y => y.Value));
        }
    }
}
