using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a defined cooldown that keeps track of duration.
    /// </summary>
    public class ResponseCooldown : ICooldown
    {
        /// <summary>
        /// Constructs a new ResponseCooldown using a specified duration.
        /// </summary>
        public ResponseCooldown(TimeSpan duration)
        {
            //complete
        }

        /// <summary>
        /// A property containing the date at which this cooldown was constructed.
        /// </summary>
        public DateTime Execution { get; set; }

        /// <summary>
        /// A property defining the length of a cooldown.
        /// </summary>
        public TimeSpan Duration { get; set; }

        public bool HasExpired { get { return Remainder.TotalMilliseconds < Duration.TotalMilliseconds; } }

        /// <summary>
        /// A TimeSpan defining the amount of time that has passed since execution.
        /// </summary>
        public TimeSpan Elapsed { get { return DateTime.UtcNow - Execution; } }

        /// <summary>
        /// A TimeSpan defining the amount of time remaining on this cooldown.
        /// </summary>
        public TimeSpan Remainder { get { return Duration - Elapsed; } }
    }

    /// <summary>
    /// Represents a defined cooldown deriving from a reward claim.
    /// </summary>
    public class RewardCooldown : ResponseCooldown, IRewardableStreak
    {
        /// <summary>
        /// Constructs a new RewardCooldown using a specified duration and optional limit.
        /// </summary>
        public RewardCooldown(TimeSpan duration, TimeSpan? limit = null) : base(duration)
        {
            if (limit.HasValue)
                Limit = limit.Value;
        }

        public ulong Reward { get; set; }
        public ulong Bonus { get; set; }

        public int Threshold { get; set; }
        public int Streak { get; set; }
        public int ThresholdStreak
        {
            get
            {
                return (int)Math.Floor((double)(Streak / Threshold));
            }
        }

        public bool MeetsThreshold
        {
            get
            {
                return (Streak % Threshold) == 0;
            }
        }

        public void Claim()
        {
            //a.Give(Reward);
        }

        public void Reset()
            => Streak = 0;

        public void Tick()
            => Streak += 1;

        private TimeSpan? _limit;
        /// <summary>
        /// A property defining the limit before a cooldown is considered as past due. If left empty, this will inherit the duration.
        /// </summary>
        public TimeSpan Limit { get { return _limit ?? TimeSpan.FromMilliseconds(Duration.TotalMilliseconds);  } set { _limit = value; } }

        public bool IsPastLimit
        {
            get
            {
                if (HasExpired)
                    return false;
                return (Elapsed - Duration).TotalMilliseconds < Limit.TotalMilliseconds;
            }
        }
    }

    /// <summary>
    /// Defines the basic properties of an object that can keep track of streaks.
    /// </summary>
    public interface IStreakable
    {
        int Streak { get; set; }
        void Reset();
        void Tick();
    }

    /// <summary>
    /// Defines the basic properties of an object that can offer rewards from a streak.
    /// </summary>
    public interface IRewardableStreak : IStreakable
    {
        /// <summary>
        /// A property defining the requirements for a bonus.
        /// </summary>
        int Threshold { get; set; }

        /// <summary>
        /// A property defining the amount of money from which is given.
        /// </summary>
        ulong Reward { get; set; }

        /// <summary>
        /// A property defining the amount of money that is given alongside its default reward.
        /// </summary>
        ulong Bonus { get; set; }

        /// <summary>
        /// A Boolean returning if the streak currently meets the threshold.
        /// </summary>
        bool MeetsThreshold { get; }

        /// <summary>
        /// A function defining a claim that returns rewards.
        /// </summary>
        void Claim();
    }
}
