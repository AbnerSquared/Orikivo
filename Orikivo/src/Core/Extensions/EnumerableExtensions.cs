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

        // shifts all of the elements of an enumerable by a specified amount
        public static IEnumerable<T> Shift<T>(this IEnumerable<T> enumerable, int shift)
        {
            // if this doesn't shift at all, just return
            if (shift == 0)
                return enumerable;

            // negative values shift left, positive values shift right
            int direction = Math.Sign(shift);
            // -1 => less than 0
            // 1 => greater than 0

            T[] array = enumerable.ToArray();

            // let's say your array is a length of 8
            // you want to shift -3
            // -3 is 3 to the left 

            // original
            // 0        1        2        3        4        5        6        7
            // array[0] array[1] array[2] array[3] array[4] array[5] array[6] array[7]

            // shift -3 (left)
            // 0        1        2        3        4        5        6        7
            // array[3] array[4] array[5] array[6] array[7] null     null     null

            // shift 3 (right)
            // 0        1        2        3        4        5        6        7
            // null     null     null     array[0] array[1] array[2] array[3] array[4]


            //         source sourceIndex dest destIndex, length
            Array.Copy(
                array, // source: What are the values I copy
                direction > 0 ? 0 : direction * shift, // sourceIndex: Where to start copying -1 * -3 => 3
                array, // destination: Where are these values being drawn
                direction > 0 ? shift : 0,     // destinationIndex: Where to start pasting
                array.Length - Math.Abs(shift)); // length: how long is this array that I am copying

            Array.Clear(array,
                // if right   | 0 OR arrayLength - abs(shift)
                direction > 0 ? 0 :  array.Length - (direction * shift), // index
                Math.Abs(shift)); // length

            return array;
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
