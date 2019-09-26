using Newtonsoft.Json;
using System;

namespace Orikivo
{
    public class UniqueItemInfo
    {

        [JsonProperty("local_id")]
        public int LocalId { get; internal set; } // to make sure items can't just goof

        [JsonProperty("trades_left")]
        public int? TradesLeft { get; internal set; }

        [JsonProperty("gifts_left")]
        public int? GiftsLeft { get; internal set; }

        [JsonProperty("uses_left")]
        public int? UsesLeft { get; internal set; }

        [JsonProperty("last_used")]
        public DateTime? LastUsed { get; internal set; }

        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; internal set; }
    }
}
