using Newtonsoft.Json;
using System;

namespace Orikivo
{
    public class MeritInfo
    {
        public MeritInfo(DateTime achievedAt, bool? claimed = null)
        {
            AchievedAt = achievedAt;
            IsClaimed = claimed;
        }

        [JsonProperty("achieved_at")]
        public DateTime AchievedAt { get; }

        [JsonProperty("is_claimed")]
        public bool? IsClaimed { get; internal set; } // null if there is nothing to claim
    }
}
