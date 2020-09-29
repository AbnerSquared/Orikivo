namespace Arcadia
{
    // This is so that a CatalogEntry can inherit base data from a group
    public class CatalogGroup
    {
        public string Id { get; set; }

        public int? MaxAllowed { get; set; }

        public int? MinDiscount { get; set; }

        public int? MaxDiscount { get; set; }

        public float DiscountChance { get; set; }

        public float CostScale { get; set; }

        public int RequiredTier { get; set; }

        public bool ForceAtTier { get; set; }
    }
}
