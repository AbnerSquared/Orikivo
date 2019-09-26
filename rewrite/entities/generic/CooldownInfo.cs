using Newtonsoft.Json;
using System;

namespace Orikivo
{
    // a basic class used to set info about a cooldown.
    public class CooldownInfo
    {
        public CooldownInfo(DateTime executedAt, double seconds)
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
