using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a custom key value comparer.
    /// </summary>
    public class KeyComparer<TKey> : IComparer<TKey>
    {
        public static KeyComparer<TKey> Default = new KeyComparer<TKey>();
        public int Compare(TKey x, TKey y)
            => ValueComparer.Default.Compare(x, y);
    }
}