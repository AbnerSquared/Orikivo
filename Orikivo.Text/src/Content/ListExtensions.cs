using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orikivo.Text
{
    public static class ListExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
            => property.GetCustomAttributes<TAttribute>().GetAttribute<TAttribute>();
        public static TAttribute GetAttribute<TAttribute>(this IEnumerable<Attribute> attributes) where TAttribute : Attribute
            => attributes.FirstOrDefault(a => a.GetType() == typeof(TAttribute)) as TAttribute;
    }
}
