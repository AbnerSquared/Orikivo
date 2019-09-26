using System;
using System.Collections;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a custom value comparer.
    /// </summary>
    public class ValueComparer : IComparer
    {
        public static ValueComparer Default = new ValueComparer();
        public static Comparer Exact = Comparer.Default;
        public static CaseInsensitiveComparer Insensitive = new CaseInsensitiveComparer();

        public int Compare(object x, object y)
            => Insensitive.Compare(x, y);

        public int ReverseCompare(object x, object y)
            => Insensitive.Compare(y, x);

        public int CompareExact(object x, object y)
            => Exact.Compare(x, y);
        
        public int ReverseCompareExact(object x, object y)
            => Exact.Compare(y, x);
    }
}