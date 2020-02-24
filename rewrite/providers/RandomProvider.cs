﻿using System;
using System.Threading;

namespace Orikivo
{
    // NOTE: This was designed by another person from StackOverflow.
    /// <summary>
    /// Represents a static provider that allows for the thread-safe usage of <see cref="Random"/>.
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

        /// <summary>
        /// Creates a new <see cref="Random"/> instance from a specified seed.
        /// </summary>
        /// <param name="seed">A 32-bit integer that represents a seed.</param>
        public static Random Seed(int seed)
        {
            lock (_lock)
                return new Random(seed);
        }

        /// <summary>
        /// Gets a new <see cref="Random"/> instance.
        /// </summary>
        public static Random Instance => _local.Value;
    }
}
