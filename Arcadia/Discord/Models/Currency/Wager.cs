namespace Arcadia
{
    public readonly struct Wager
    {
        public Wager(long value, CurrencyType currency = CurrencyType.Token)
        {
            Value = value;
            Currency = currency;
        }

        public long Value { get; }

        public CurrencyType Currency { get; }
    }
}
