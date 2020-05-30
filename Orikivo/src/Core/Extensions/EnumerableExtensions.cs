using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class EnumerableExtensions
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryAdd(key, value))
                dictionary[key] = value;
        }

        // shifts all of the elements of the list left by the specific amount.
        // 
        // items2 = items.ShiftLeft(1);
        // example: items2[0] == items[1]; 
        // referred from stackoverflow
        // https://stackoverflow.com/questions/18180958/does-code-exist-for-shifting-list-elements-to-left-or-right-by-specified-amount
        public static IEnumerable<T> ShiftLeft<T>(this IEnumerable<T> list, int count)
        {
            T[] arr = list.ToArray();
            Array.Copy(arr, count, arr, 0, arr.Length - count);
            Array.Clear(arr, arr.Length - count, count);

            return arr;
        }

        /// <summary>
        /// Returns a new <see cref="List{T}"/> in which all of the inner lists are merged together.
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> set)
        {
            var result = new List<T>();

            foreach (IEnumerable<T> list in set)
                result.AddRange(list);

            return result;
        }

        public static void AddRange<T>(this List<T> list, params T[] ts)
        {
            if (ts.Length > 0)
                list.AddRange(ts);
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }

        /// <summary>
        /// Performs the specified action on each element (with its provided index) of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int i = 0;
            foreach (T item in source)
            {
                action(item, i);
                i++;
            }
        }

        // TEST THIS OUT ONCE ABLE, could replace FindAttributes<T> and FirstAttribute<T>
        public static IEnumerable<TValue> OfType<TSource, TValue>(this IEnumerable<TSource> enumerable)
            where TSource : class
            where TValue : class, TSource
            => enumerable.Where(x => x.GetType() == typeof(TValue)).Select(x => x as TValue);

        // ensures that the underlying type is as specified.
        public static IEnumerable<TAttribute> FindAttributes<TAttribute>(this IEnumerable<Attribute> attributes)
            where TAttribute : Attribute
            => attributes.Where(x => x.GetType() == typeof(TAttribute)).Select(x => x as TAttribute);

        public static TAttribute FirstAttribute<TAttribute>(this IEnumerable<Attribute> attributes)
            where TAttribute : Attribute
            => attributes.FirstOrDefault(a => a.GetType() == typeof(TAttribute)) as TAttribute;
    }
}
