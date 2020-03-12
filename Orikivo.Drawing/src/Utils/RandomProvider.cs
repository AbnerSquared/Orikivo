using System;
using System.Threading;

namespace Orikivo.Drawing
{
    // TODO: Create shared utility classes.
    /// <summary>
    /// A static provider that allows for thread-safe random construction.
    /// </summary>
    public static class RandomProvider
    {
        private static readonly Random _global = new Random();
        private static readonly object _lock = new object();
        private static readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(New);

        /// <summary>
        /// Creates a new instance of random from an internal static Random.
        /// </summary>
        public static Random New()
        {
            lock (_lock)
                return new Random(_global.Next());
        }

        public static Random Instance => _local.Value;
    }
}
