using Newtonsoft.Json;
using System;

namespace Arcadia
{
    /// <summary>
    /// Represents the data of a <see cref="Merit"/>.
    /// </summary>
    public class MeritData
    {
        [JsonConstructor]
        internal MeritData(DateTime achievedAt, bool? isClaimed = null)
        {
            AchievedAt = achievedAt;
            IsClaimed = isClaimed;
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Merit"/> was achieved.
        /// </summary>
        [JsonProperty("achieved_at")]
        public DateTime AchievedAt { get; }

        /// <summary>
        /// Gets a <see cref="bool"/> that specifies if the <see cref="Merit"/> has been claimed. If unspecified, the <see cref="Merit"/> does not have a reward.
        /// </summary>
        [JsonProperty("claimed")]
        public bool? IsClaimed { get; internal set; }
    }
}
