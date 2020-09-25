namespace Arcadia
{
    public class ValueInfo
    {
        public ValueInfo() {}

        public ValueInfo(CurrencyType currency, long value, ShopMode mode = ShopMode.Any)
        {
            Currency = currency;
            Value = value;
            AllowedModes = mode;
        }

        public CurrencyType Currency { get; set; }

        public long Value { get; set; }

        public ShopMode AllowedModes { get; set; }
    }
}
