using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public static class ListExtensions
    {
        public static bool TryGetElementAt<T>(this List<T> list, int index, out T t)
        {
            t = default;

            if (index >= 0 && index <= list.Count - 1)
            {
                t = list.ElementAt(index);
                return true;
            }

            return false;
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> source) where T : struct
        {
            if (source.Count() > 0)
                return source.First();

            return null;
        }

        public static TResult CastObject<TResult>(this object obj)
        {
            if (obj is TResult)
                return (TResult)obj;

            try
            {
                return (TResult)Convert.ChangeType(obj, typeof(TResult));
            }
            catch(InvalidCastException)
            {
                return default;
            }
        }
    }
}
