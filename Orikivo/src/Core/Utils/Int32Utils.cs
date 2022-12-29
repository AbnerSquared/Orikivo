using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class NumberUtils
    {
        public static IEnumerable<int> CreateInt32Range(int start, int end, int increment = 1)
        {
            if (increment == 0)
                throw new ArgumentException("The specified increment amount must not be equal to 0.");

            return Enumerable.Repeat(start, ((end - start) / increment) + 1).Select((u, v) => u + (increment * v));
        }

        public static IEnumerable<int> CreateInt32Range(int start, int end, int increment, HashSet<int> exclude)
        {
            if (increment == 0)
                throw new ArgumentException("The specified increment amount must not be equal to 0.");

            return Enumerable
                .Repeat(start, ((end - start) / increment) + 1)
                .Select((u, v) => u + (increment * v))
                .Where(x => !exclude.Contains(x));
        }
    }
}
