using System;

namespace Orikivo
{
    /// <summary>
    /// Represents the advanced configuration for a <see cref="MessageCollector"/>.
    /// </summary>
    public class MatchOptions
    {
        /// <summary>
        /// Gets the default <see cref="MatchOptions"/>.
        /// </summary>
        public static MatchOptions Default => new MatchOptions { ResetTimeoutOnAttempt = false, Timeout = TimeSpan.FromSeconds(10) };
        
        /// <summary>
        /// The amount of time that is allowed to pass before the <see cref="MessageCollector"/> closes.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Determines if the <see cref="MessageCollector"/> should reset its timeout on any attempt.
        /// </summary>
        public bool ResetTimeoutOnAttempt { get; set; }

        /// <summary>
        /// The action that will be applied on each <see cref="FilterMatch"/> when an attempt is successful.
        /// </summary>
        public MatchAction Action { get; set; }
    }
}
