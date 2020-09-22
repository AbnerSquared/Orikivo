namespace Arcadia
{
    public class CatalogEntry
    {
        public static readonly int DefaultMinDiscount = 1;
        public static readonly int DefaultMaxDiscount = 25;

        public CatalogEntry() { }

        public CatalogEntry(string itemId, int weight, int? maxAllowed = null)
        {
            ItemId = itemId;
            Weight = weight;
            MaxAllowed = maxAllowed;
        }

        public string ItemId { get; set; }

        public int? MaxAllowed { get; set; }

        public int? MinDiscount { get; set; }

        public int? MaxDiscount { get; set; }

        public float DiscountChance { get; set; }

        // The rate that this item is sold at in comparison
        // due to its rarity
        public float CostScale { get; set; }

        // The minimum tier you need to be for this shop to show this entry
        public int RequiredTier { get; set; } = 1;

        // If true, this entry is excluded where the user is not at the specified shop tier
        public bool ForceAtTier { get; set; }

        public bool IsSpecial { get; set; }

        public int Weight { get; set; }
    }
}
