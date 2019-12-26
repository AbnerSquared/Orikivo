using System.Collections.Generic;
using System.Linq;
using System;
using static System.Math;

namespace Orikivo
{
    /// <summary>
    /// An expansion on mathematical functions for various numbers and sets.
    /// </summary>
    public static class OriMath
    {
        /// <summary>
        /// Calculates the average of all numbers within a set.
        /// </summary>
        public static double Mean(IEnumerable<int> set)
            => set.Sum() / set.Count();

        /// <summary>
        /// Divides a number only based on how many times the number can fit in without decimals.
        /// </summary>
        public static int DivideWhole(int dividend, int divisor)
        {
            int quotient = 0;
            while(dividend % divisor == 0)
            {
                dividend -= divisor;
                quotient++;
            }
            return quotient;
        }

        /// <summary>
        /// Returns the value specified to the power of 2.
        /// </summary>
        public static double Squared(double x)
            => Pow(x, 2);

        /// <summary>
        /// Returns the root n of a value specified.
        /// </summary>
        public static double Root(double x, double n)
            => Pow(10, Log10(x) / 2);

        /// <summary>
        /// Returns the remainder of a subtraction between two UInt64 values. If the minuend can take itself from the subtrahend, with the subtrahend being greater than zero, it returns zero.
        /// </summary>
        /// <param name="subtrahend">The number being subtracted.</param>
        /// <param name="minuend">The number that the subtrahend will be subtracted by.</param>
        public static ulong SubtractRem(ulong subtrahend, ulong minuend)
            => subtrahend - minuend < 0 ? minuend - subtrahend : 0;

        /// <summary>
        /// Returns the exact median value from a set of numbers.
        /// </summary>
        public static double MedianValue(IEnumerable<int> set)
            => Median(set).Sum() / 2;

        /// <summary>
        /// Returns all median values from a set of numbers. If the set has an even count, it returns two median values.
        /// </summary>
        public static List<int> Median(IEnumerable<int> set)
        {
            List<int> middles = new List<int>();
            int parity = Parity(set.Count());
            int median = (parity == 1 ? set.Count() + 1 : set.Count()) / 2;
            int values = parity + 1; // 1 value if odd, 2 if even

            for (int i = 0; i < values; i++)
                middles.Add(median + i);
            return middles;
        }

        /// <summary>
        /// Returns a number defining if the number is even (0) or odd (1).
        /// </summary>
        public static int Parity(int number)
            => number % 2;

        /// <summary>
        /// Returns all numbers from a set that matches the highest occurrances found.
        /// </summary>
        public static List<int> Mode(IEnumerable<int> set) // TODO: Add Grouping functions, maybe with Range
        {
            Dictionary<int, int> setCounter = new Dictionary<int, int>();
            foreach (int number in set)
            {
                if (setCounter.ContainsKey(number))
                    setCounter[number] += 1;
                else
                    setCounter[number] = 1;
            }
            // Groups key value pairs by the occurance of the number. it then orders the groups by descending using the occurrance, selects the first
            // occurrance, as it is the highest, and selects the keys (which are the numbers) from the group into a list.
            return setCounter.GroupBy(x => x.Value).OrderByDescending(x => x.Key).First().Select(x => x.Key).ToList();
        }
    }
}
