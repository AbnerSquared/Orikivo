using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class Int32Utils
    {
        public static IEnumerable<int> Increment(int start, int end, int increment = 1)
        {
            if (increment == 0)
                throw new DivideByZeroException("Dividing by zero is a sin.");
            return Enumerable.Repeat(start, (int)((end - start) / increment) + 1).Select((u, v) => u + (increment * v));
        }
    }
}
