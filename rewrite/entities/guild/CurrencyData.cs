namespace Orikivo
{
    /// <summary>
    /// Represents data for a currency with a specified type and value.
    /// </summary>
    public class CurrencyData
    {
        internal CurrencyData(CurrencyType type = CurrencyType.Generic)
        {
            Type = type;
            Value = 0;
            Debt = 0;
        }

        public CurrencyType Type { get; }

        /// <summary>
        /// Represents the value of the specified type of currency.
        /// </summary>
        public ulong Value { get; protected set; }

        /// <summary>
        /// Represents the 'Etiro' value of this currency.
        /// </summary>
        public ulong Debt { get; protected set; }

        public long Net => (long)Value - (long)Debt;

        internal void Give(ulong value)
        {
            ulong remainder = OriMath.Subtract(Debt, value);
            Debt -= value - remainder;
            if (remainder > 0)
                Value += value;
        }

        internal void Take(ulong value)
        {
            ulong remainder = OriMath.Subtract(Value, value);
            Value -= value - remainder;
            if (remainder > 0)
                Debt += remainder;
        }

        /// <summary>
        /// Attempts to add a value into the currency data, focusing on the debt first.
        /// </summary>
        public static CurrencyData operator +(CurrencyData data, ulong value)
        {
            data.Give(value);
            return data;
        }

        public static CurrencyData operator -(CurrencyData data, ulong value)
        {
            data.Take(value);
            return data;
        }

        public static bool operator ==(CurrencyData left, CurrencyData right)
        {
            if (left.Type != right.Type)
                return false;
            if (left.Net == right.Net)
                return true;
            return false;
        }

        public static bool operator !=(CurrencyData left, CurrencyData right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (GetType() == obj.GetType())
                return this == obj as CurrencyData;
            return false;
        }

        // << == a * 2^(n), >> == a / 2^(n)
        public override int GetHashCode()
            => unchecked((int)Value - (int)Debt << (int)Type);
    }
}
