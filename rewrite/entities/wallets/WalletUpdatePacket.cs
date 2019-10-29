using Newtonsoft.Json;

namespace Orikivo
{
    public class WalletUpdatePacket
    {
        [JsonConstructor]
        internal WalletUpdatePacket(ulong? guildId, CurrencyType type, UpdateMethod method, ulong amount)
        {
            GuildId = guildId;
            Type = type;
            Method = method;
            Amount = amount;
        }
        
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; }

        [JsonProperty("type")]
        public CurrencyType Type { get; }

        [JsonProperty("method")]
        public UpdateMethod Method { get; }

        [JsonProperty("amount")]
        public ulong Amount { get; }
    }
}
