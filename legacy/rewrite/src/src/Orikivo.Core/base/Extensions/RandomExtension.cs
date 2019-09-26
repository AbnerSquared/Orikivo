using System;
using System.Collections.Generic;
using Orikivo.Helpers;

namespace Orikivo
{
    public static class RandomExtension
    {
        public static int Roll (this Random rng)
            => RandomHelper.Roll(rng);

        public static int Roll (this Random rng, Dice d)
            => RandomHelper.Roll(rng, d);

        public static bool Flip (this Random rng)
            => RandomHelper.FlipCoin(rng);

        public static List<int> RollMany(this Random rng, Dice d, int i)
            => RandomHelper.RollMany(rng, d, i);

        /// <summary>
        /// Groups up all of the integers into a single number.
        /// </summary>
        /// <returns>Returns a ulong in which all integers are added together.</returns>
        public static ulong Tally(this List<int> ints)
        {
            ulong r = 0;
            foreach (int i in ints)
            {
                r += (ulong)i;
            }

            return r;
        }


        public static bool IsInRange(this int i, Range r)
            => i.IsInRange(r.Min, r.Max);

        public static bool IsInRange(this int i, int max)
        {
            return i == i.InRange(max);
        }

        public static bool IsInRange(this int i, int min, int max)
        {
            return i == i.InRange(min, max);
        }
        
    }
}