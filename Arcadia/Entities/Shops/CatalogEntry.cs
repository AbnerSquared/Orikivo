namespace Arcadia
{
    public class CatalogEntry
    {
        public static readonly int DefaultMinDiscount = 1;
        public static readonly int DefaultMaxDiscount = 25;

        public string ItemId { get; set; }

        public int? MaxAllowed { get; set; }

        public int? MinDiscount { get; set; }

        public int? MaxDiscount { get; set; }

        public float DiscountChance { get; set; }

        public bool IsSpecial { get; set; }

        public int Weight { get; set; }
    }
}
