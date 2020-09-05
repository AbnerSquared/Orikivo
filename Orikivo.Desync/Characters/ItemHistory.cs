using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    internal static class EnumerableExtensions
    {
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
                direction > 0 ? 0 : array.Length - (direction * shift), // index
                Math.Abs(shift)); // length

            return array;
        }
    }

    public class ItemHistory
    {
        private readonly int _capacity;

        public ItemHistory()
        {
            _capacity = 32;
        }

        public IEnumerable<ItemLog> Entries { get; set; }

        public void Append(ItemLog log)
        {
            if (Entries.Count() >= _capacity)
                Entries = Entries.Shift(1);

            Entries = Entries.Prepend(log);
        }
    }
}
