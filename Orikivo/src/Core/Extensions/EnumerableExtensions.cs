using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Shifts all of the elements within an <see cref="IEnumerable{T}"/> by a specified amount.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to shift.</param>
        /// <param name="shift">The amount to shift by. Negative values shift left.</param>
        public static IEnumerable<T> Shift<T>(this IEnumerable<T> enumerable, int shift)
        {
            // If this doesn't shift at all, just return the enumerable as is.
            if (shift == 0)
                return enumerable;

            int direction = Math.Sign(shift);
            T[] array = enumerable.ToArray();

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

        // TODO: Implement recursive flattening.
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

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="List{T}"/>.
        /// </summary>
        public static void AddRange<T>(this List<T> list, params T[] args)
        {
            if (args.Length > 0)
                list.AddRange(args);
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
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>, with the second parameter providing its index.
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

        /// <summary>
        /// Returns the first element of a sequence that matches a specified type, or a default value if the sequence contains no elements.
        /// </summary>
        public static TResult FirstOrDefault<TResult>(this IEnumerable enumerable)
            => enumerable.OfType<TResult>().FirstOrDefault();

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        public static IEnumerable<KeyValuePair<TKey, TValue>> Where<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, Func<TKey, TValue, bool> predicate)
            => enumerable.Where(x => predicate.Invoke(x.Key, x.Value));

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        public static IEnumerable<KeyValuePair<TKey, TValue>> Where<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, Func<TKey, TValue, int, bool> predicate)
            => enumerable.Where((x, i) => predicate.Invoke(x.Key, x.Value, i));

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
            => enumerable.ToDictionary(x => x.Key, x => x.Value);
    }
}
