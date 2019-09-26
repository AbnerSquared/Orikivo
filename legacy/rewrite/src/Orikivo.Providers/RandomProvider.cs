using System;
using System.Threading;

namespace Orikivo.Providers
{
    public static class RandomProvider
    {
        private static readonly Random _global = new Random();
        private static readonly object _lock = new object();
        private static readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(New);

        public static Random New()
        {
            lock(_lock)
                return new Random(_global.Next());
        }

        public static Random Instance
        {
            get
            {
                return _local.Value;
            }
        }
    }
}