using System;

namespace Orikivo
{
    /// <summary>
    /// Represents advanced configuration for a <see cref="MessageCollector"/>.
    /// </summary>
    public class SessionOptions
    {
        /// <summary>
        /// Gets the default <see cref="SessionOptions"/>.
        /// </summary>
        public static readonly SessionOptions Default = new SessionOptions
        {
            ResetTimeoutOnAttempt = false,
            Timeout = TimeSpan.FromSeconds(10)
        };

        /// <summary>
        /// The amount of time that is allowed to pass before the <see cref="MessageCollector"/> closes.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Determines if the <see cref="MessageCollector"/> should reset its timeout on any attempt.
        /// </summary>
        public bool ResetTimeoutOnAttempt { get; set; }

        /// <summary>
        /// Gets or sets a message session that will handle all successful <see cref="FilterMatch"/> results.
        /// </summary>
        public MessageSession Session { get; set; }
    }
}
