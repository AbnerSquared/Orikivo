using Newtonsoft.Json;
using System;

namespace Orikivo
{

    public class MeritData
    {
        [JsonConstructor]
        internal MeritData(string id, DateTime achievedAt, bool? claimed = null)
        {
            Id = id;
            AchievedAt = achievedAt;
            IsClaimed = claimed;
        }

        /// <summary>
        /// The unique identifier of the merit.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The time that the user achieved this merit.
        /// </summary>
        [JsonProperty("achieved_at")]
        public DateTime AchievedAt { get; }

        /// <summary>
        /// A check defining if the merit was already claimed. If null, there isn't anything to claim.
        /// </summary>
        [JsonProperty("is_claimed")]
        public bool? IsClaimed { get; internal set; }
    }
}
