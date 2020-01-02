﻿using System;
using System.Threading;

namespace Orikivo
{
    // NOTE: This was designed by another person from StackOverflow.
    /// <summary>
    /// A static provider that allows for the thread-safe usage of <see cref="Random"/>.
    /// </summary>
    public static class RandomProvider
    {
        private static readonly Random _global = new Random();
        private static readonly object _lock = new object();
        private static readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(New);

        /// <summary>
        /// Creates a new <see cref="Random"/> instance from a seed generated by its global <see cref="Random"/>.
        /// </summary>
        public static Random New()
        {
            lock (_lock)
                return new Random(_global.Next());
        }

        public static Random FromSeed(int seed)
        {
            lock (_lock)
                return new Random(seed);
        }

        public static Random Instance => _local.Value;
    }
}
