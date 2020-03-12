using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    internal static class Int32Extensions
    {
        public static bool IsInRange(this int i, int max)
            => i <= max - 1 && i >= 0;
        public static bool IsInRange(this int i, int min, int max)
            => i <= max - 1 && i >= min;

        // Creates an array starting from 0 up (or down) to int i.
        public static IEnumerable<int> Increment(this int max)
            => Int32Utils.Increment(0, Math.Abs(max), Math.Sign(max));
    }
}
