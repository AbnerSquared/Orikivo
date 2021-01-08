namespace Arcadia
{
    internal static class Icons
    {
        internal static string IconOf(BoostTarget type)
        {
            return type switch
            {
                BoostTarget.Money => Balance,
                BoostTarget.Debt => Debt,
                BoostTarget.Chips => Chips,
                BoostTarget.Voting => Tokens,
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
        internal static readonly string Notice = "🔔";
        internal static readonly string Skip = "⏭️";
        internal static readonly string Quests = "📜";
        internal static readonly string Challenges = "🧧";

        private const string CLOCK_1 = "🕐";
        private const string CLOCK_2 = "🕑";
        private const string CLOCK_3 = "🕒";
        private const string CLOCK_4 = "🕓";
        private const string CLOCK_5 = "🕔";
        private const string CLOCK_6 = "🕕";
        private const string CLOCK_7 = "🕖";
        private const string CLOCK_8 = "🕗";
        private const string CLOCK_9 = "🕘";
        private const string CLOCK_10 = "🕙";
        private const string CLOCK_11 = "🕚";
        private const string CLOCK_12 = "🕛";

        public static string GetClock(int hour)
        {
            switch (hour)
            {
                case 11:
                case 23:
                    return CLOCK_11;

                case 10:
                case 22:
                    return CLOCK_10;

                case 9:
                case 21:
                    return CLOCK_9;

                case 8:
                case 20:
                    return CLOCK_8;

                case 7:
                case 19:
                    return CLOCK_7;

                case 6:
                case 18:
                    return CLOCK_6;

                case 5:
                case 17:
                    return CLOCK_5;

                case 4:
                case 16:
                    return CLOCK_4;

                case 3:
                case 15:
                    return CLOCK_3;

                case 2:
                case 14:
                    return CLOCK_2;

                case 1:
                case 13:
                    return CLOCK_1;

                default:
                    return CLOCK_12;
            }
        }
    }
}
