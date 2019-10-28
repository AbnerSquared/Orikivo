namespace Orikivo
{
    /// <summary>
    /// Represents data about a currency for a specified guild.
    /// </summary>
    public class GuildCurrencyData : CurrencyData
    {
        public GuildCurrencyData(ulong guildId) : base(CurrencyType.Guild)
        {
            GuildId = guildId;
        }

        public ulong GuildId { get; }

        public static GuildCurrencyData operator +(GuildCurrencyData data, ulong value)
        {
            data.Give(value);
            return data;
        }

        public static GuildCurrencyData operator -(GuildCurrencyData data, ulong value)
        {
            // the most that can be removed.
            data.Take(value);
            return data;
        }
    }
}
