namespace Orikivo.Desync
{
    public class LootEntry : ITableEntry
    {
        /// <summary>
        /// Defines the action required in order to receive this entry.
        /// </summary>
        public ExploreAction Action { get; set; }

        public string ItemId { get; set; }

        public ItemType Groups { get; set; }

        public int Weight { get; set; }
    }
}
