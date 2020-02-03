using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Claimable
    {
        // TODO: Fix Id storage in Claimable
        // TODO: Create Rewards based on looping requirements (modulo)
        public string Id { get; set; }

        /// <summary>
        /// The unique identifier specified in <see cref="Stats"/> that will be updated based on this current streak.
        /// </summary>
        public string StreakId { get; set; }

        /// <summary>
        /// The amount of time a <see cref="User"/> has to wait before being able to claim.
        /// </summary>
        public TimeSpan Cooldown { get; set; }

        /// <summary>
        /// The amount of time a streak can go before being reset.
        /// </summary>
        public TimeSpan PreserveDuration { get; set; }

        /// <summary>
        /// Specifies a collection of <see cref="Reward"/> values bound to a streak amount.
        /// </summary>
        public Dictionary<int, Reward> RewardTiers { get; set; } = new Dictionary<int, Reward>();
    }
}
