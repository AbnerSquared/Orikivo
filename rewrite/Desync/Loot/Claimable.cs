using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a cooldown with streak tracking.
    /// </summary>
    public class Claimable
    {
        // TODO: Fix Id storage in Claimable
        // TODO: Create Rewards based on looping requirements (modulo)
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets unique identifier specified in <see cref="Stats"/> that will be updated based on this current streak.
        /// </summary>
        public string StreakId { get; set; }

        /// <summary>
        /// Gets or sets amount of time a <see cref="User"/> has to wait before being able to claim.
        /// </summary>
        public TimeSpan Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a streak can go before being reset.
        /// </summary>
        public TimeSpan Preservation { get; set; }

        /// <summary>
        /// Specifies a collection of <see cref="Reward"/> values bound to a streak amount.
        /// </summary>
        public Dictionary<int, Reward> RewardTiers { get; set; } = new Dictionary<int, Reward>();
    }

    public enum ClaimFlag
    {
        /// <summary>
        /// Marks the <see cref="Claimable"/> to loop its rewards.
        /// </summary>
        Modulo = 1,

        /// <summary>
        /// Marks the <see cref="Claimable"/> to allow rewards only once.
        /// </summary>
        Once = 2
    }
}
