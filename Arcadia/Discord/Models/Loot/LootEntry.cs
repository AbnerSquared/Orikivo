namespace Arcadia
{
    public class LootEntry
    {
        public LootEntry() { }

        public LootEntry(long money, CurrencyType currency)
        {
            Money = money;
            Currency = currency;
        }

        public LootEntry(string itemId, int weight)
        {
            ItemId = itemId;
            Weight = weight;
        }

        public string ItemId { get; set; }

        // If specified, overrides the item ID
        public long Money { get; set; } // Replace with Wager thing

        public CurrencyType Currency { get; set; } = CurrencyType.Cash;

        public int Weight { get; set; }

        public StackRange Stack { get; set; } = 1;
    }
}
