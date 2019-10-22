using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class TExtensions
    {
        public static bool EqualsAny<T>(this T obj, params T[] args)
            => args.Contains(obj);

        /// <summary>
        /// Creates a new list with itself being the first element.
        /// </summary>
        public static List<T> CreateList<T>(this T t)
            => new List<T>() { t };
    }
}
