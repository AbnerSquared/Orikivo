using Newtonsoft.Json;
using System;

namespace Arcadia
{
    /// <summary>
    /// Represents the data of a <see cref="Badge"/>.
    /// </summary>
    public class BadgeData
    {
        [JsonConstructor]
        internal BadgeData(DateTime unlockedAt, bool? isClaimed = null)
        {
            UnlockedAt = unlockedAt;
            IsClaimed = isClaimed;
        }

        // TODO: Catch and rename ALL instances of achieved_at to unlocked_at
        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the <see cref="Badge"/> was unlocked.
        /// </summary>
        [JsonProperty("unlocked_at")]
        public DateTime UnlockedAt { get; }

        /// <summary>
        /// Gets a <see cref="bool"/> that specifies if the <see cref="Badge"/> has been claimed. If unspecified, the <see cref="Badge"/> does not have a reward.
        /// </summary>
        [JsonProperty("claimed")]
        public bool? IsClaimed { get; internal set; }
    }
}
