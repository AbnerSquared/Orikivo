using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class VariantGenericExtensions
    {
        /// <summary>
        /// Returns a value whether any of the specified objects have the same value as this current instance.
        /// </summary>
        public static bool EqualsAny<T>(this T obj, params T[] args)
            => args.Contains(obj);

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that contains this current instance.
        /// </summary>
        public static List<T> AsList<T>(this T obj)
            => new List<T> { obj };
    }
}
