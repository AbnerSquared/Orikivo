namespace Arcadia
{
    public class Wager
    {
        public Wager(long value, CurrencyType currency = CurrencyType.Chips)
        {
            Value = value;
            Currency = currency;
        }

        // public ulong UserId { get; }

        public long Value { get; }

        public CurrencyType Currency { get; }
    }
}