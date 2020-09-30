using System;

namespace Orikivo
{
    /// <summary>
    /// Represents the basic configuration when using <see cref="MessageCollector.CollectAsync"/>.
    /// </summary>
    public class CollectionOptions
    {
        /// <summary>
        /// Gets the default <see cref="CollectionOptions"/>.
        /// </summary>
        public static readonly CollectionOptions Default = new CollectionOptions
        {
            Capacity = null,
            IncludeFailedMatches = false,
            Timeout = TimeSpan.FromSeconds(30),
            ResetTimeoutOnMatch = false
        };

        /// <summary>
        /// The amount of time that is allowed to pass without a new message before automatically closing.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Determines if the specified timeout should reset on a successful match.
        /// </summary>
        public bool ResetTimeoutOnMatch { get; set; } = false;

        /// <summary>
        /// Determines the amount of successful matches it should reach before closing.
        /// </summary>
        public int? Capacity { get; set; } = null;

        /// <summary>
        /// Determines if failed matches should be included when both resetting the timeout and capacity.
        /// </summary>
        public bool IncludeFailedMatches { get; set; } = false;
    }
}
