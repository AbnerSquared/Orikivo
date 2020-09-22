﻿namespace Arcadia
{
    public class LootEntry
    {
        public LootEntry() { }

        public LootEntry(string itemId, int weight)
        {
            ItemId = itemId;
            Weight = weight;
        }

        public string ItemId { get; set; }

        public int Weight { get; set; }
    }
}