namespace Arcadia
{
    internal static class Icons
    {
        internal static string IconOf(BoosterType type)
        {
            return type switch
            {
                BoosterType.Money => Balance,
                BoosterType.Debt => Debt,
                BoosterType.Chips => Chips,
                BoosterType.Voting => Tokens,
                _ => Unknown
            };
        }

        internal static string IconOf(CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Money => Balance,
                CurrencyType.Chips => Chips,
                CurrencyType.Tokens => Tokens,
                CurrencyType.Debt => Debt,
                _ => Unknown
            };
        }

        internal static readonly string Unknown = "UNKNOWN_ICON";
        internal static readonly string Complete = "🧤";
        internal static readonly string Assign = "🗺️";
        internal static readonly string Booster = "🧃";
        internal static readonly string Palette = "🧪";
        internal static readonly string Summon = "🎫";
        internal static readonly string Balance = "💸";
        internal static readonly string Chips = "🧩";
        internal static readonly string Tokens = "🏷️";
        internal static readonly string Debt = "📃";
        internal static readonly string Exp = "🔺";
        internal static readonly string Warning = "⚠️";
        internal static readonly string Tooltip = "🛠️";
        internal static readonly string Announcement = "📯";
        internal static readonly string Deny = "🚫";
        internal static readonly string Inventory = "📂";
        internal static readonly string Gift = "🎁";
    }
}