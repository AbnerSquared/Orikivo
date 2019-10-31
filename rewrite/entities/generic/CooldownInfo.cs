using Newtonsoft.Json;
using System;

namespace Orikivo
{
    // TODO: This can be made for checking cooldowns, but creating cooldowns only need one value: ExpiresOn. This saves space.
    public class CooldownCheckInfo
    {
        public CooldownCheckInfo(DateTime executedAt, double seconds)
        {
            ExecutedAt = executedAt;
            Seconds = seconds;
        }

        [JsonProperty("executed_at")]
        public DateTime ExecutedAt { get; }

        [JsonProperty("seconds")]
        public double Seconds { get; }

        [JsonIgnore]
        public DateTime ExpiresOn => ExecutedAt.AddSeconds(Seconds);

        [JsonIgnore]
        public TimeSpan TimeLeft => ExpiresOn - DateTime.UtcNow;

        [JsonIgnore]
        public bool IsActive => (DateTime.UtcNow - ExecutedAt).TotalSeconds < Seconds;
    }
}
