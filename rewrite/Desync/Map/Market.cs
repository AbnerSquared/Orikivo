using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    public class Market
    {
        public string Id { get; set; }
        public string Name { get; set; }

        // what the outside of the market looks like
        public string ExteriorImagePath { get; set; }

        // what the inside of the market looks like
        public string InteriorImagePath { get; set; }
        
        // a generic list of tags that this market sells
        public LootTable Table { get; set; } // the tags of the groups of items that it can sell.
        
        // a list of workers that work here
        public List<Vendor> Vendors { get; set; }

        // marked true if the market supports the buying of items
        public bool CanBuyFrom { get; set; }

        // marked true if the markets supports the selling of items
        public bool CanSellFrom { get; set; }
        
        // a multiplier applied to items that are sold
        public float SellRate { get; set; }


        // creates a new list of items to sell for the day
        public MarketCatalog GenerateCatalog()
        {
            IEnumerable<Item> loot = GameDatabase.Items
                .Where(x => Table.Groups.Any(t => x.Value.Tag.HasFlag(t)))
                .Where(x => x.Value.Tag.HasFlag(Table.RequiredTags))
                .Select(x => x.Value);

            List<Item> catalog = Randomizer.ChooseMany(loot, Table.Capacity, loot.Count() < Table.Capacity).ToList();

            return new MarketCatalog(catalog);
        }

        // Get schedule from Vendor shifts.
    }

    public class MarketCatalog
    {
        public MarketCatalog(List<Item> items)
        {
            Items = items;
            GeneratedAt = DateTime.UtcNow;
        }

        public DateTime GeneratedAt { get; set; }
        public List<Item> Items { get; }
    }

    public class LootTable
    {
        // these tags are required to be on the items.
        public ItemTag RequiredTags { get; set; }

        // as long as one of these tags are on the item, it counts
        public List<ItemTag> Groups { get; set; }

        // the most amount of items a market can hold at a time.
        public int Capacity { get; set; }
    }
}
