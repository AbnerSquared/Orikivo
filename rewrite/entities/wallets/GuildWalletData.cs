using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents data about a currency for a specified guild.
    /// </summary>
    public class GuildWalletData : WalletData
    {
        [JsonConstructor]
        internal GuildWalletData(ulong guildId, CurrencyType type, ulong value, ulong debt) : base(type)
        {
            GuildId = guildId;
            Value = value;
            Debt = debt;
        }

        public GuildWalletData(ulong guildId) : base(CurrencyType.Guild)
        {
            GuildId = guildId;
        }

        public ulong GuildId { get; }

        public static GuildWalletData operator +(GuildWalletData data, ulong value)
        {
            data.Give(value);
            return data;
        }

        public static GuildWalletData operator -(GuildWalletData data, ulong value)
        {
            // the most that can be removed.
            data.Take(value);
            return data;
        }
    }
}
