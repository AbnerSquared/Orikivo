using Newtonsoft.Json;
using System;

namespace Orikivo
{

    public class MeritData
    {
        public MeritData(DateTime achievedAt, bool? claimed = null)
        {
            AchievedAt = achievedAt;
            IsClaimed = claimed;
        }

        /// <summary>
        /// The time that the user achieved this merit.
        /// </summary>
        [JsonProperty("achieved_at")]
        public DateTime AchievedAt { get; }

        /// <summary>
        /// A check defining if the merit was already claimed. If null, the merit doesn't have anything to claim.
        /// </summary>
        [JsonProperty("is_claimed")]
        public bool? IsClaimed { get; internal set; } // null if there is nothing to claim
    }
}
