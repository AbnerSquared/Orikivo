using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class EnumerableExtensions
    {
        // DictionaryExtensions
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
        public static List<T> ShiftLeft<T>(this List<T> list, int count, bool wrap = false) // if the elements wrap around its current capacity.
        {
            T[] arr = list.ToArray();
            Array.Copy(arr, count, arr, 0, arr.Length - count);
            Array.Clear(arr, arr.Length - count, count);
            return arr.ToList();
            // add a wrapping ability
        }

        /// <summary>
        /// Returns a new <see cref="List{T}"/> in which all of the inner lists are merged together.
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> set)
        {
            List<T> result = new List<T>();
            foreach (List<T> list in set)
                result.AddRange(list);

            return result;
        }

        public static void AddRange<T>(this List<T> list, params T[] ts)
        {
            if (ts.Length > 0)
                list.AddRange(ts.ToList());
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int i = 0;
            foreach (T item in source)
            {
                action(item, i);
                i++;
            }
        }

        // ensures that the underlying type is as specified.
        public static IEnumerable<TAttribute> FindAttributes<TAttribute>(this IEnumerable<Attribute> attributes) where TAttribute : Attribute
            => attributes.Where(x => x.GetType() == typeof(TAttribute)).Select(x => x as TAttribute);

        public static TAttribute FindAttribute<TAttribute>(this IEnumerable<Attribute> attributes) where TAttribute : Attribute
            => attributes.FirstOrDefault(a => a.GetType() == typeof(TAttribute)) as TAttribute;
        // => attributes.FirstOrDefault(a => a is TAttribute) as TAttribute;
    }
}
