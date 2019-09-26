using Orikivo.Cache;
using Orikivo.Utility;
using System;

namespace Orikivo
{
    public static class RangeHelper
    {
        public static double Shift(int num, Range from, Range to)
        {
            // make sure the number is in range before going.
            if (!num.IsInRange(from))
                throw new Exception(new OriError()["RangeOutOfAreaException"]);

            return Calculator.RangeShift(num, from.Min, from.Max, to.Min, to.Max);
        }
    }

    public class Range
    {
        public Range(int min, int max)
        {
            if (min == max)
                throw new Exception(new OriError()["RangeNullException"]);
            Min = min > max ? max : min;
            Max = max < min ? min : max;
        }

        public int Min { get; }
        public int Max { get; }
        public int Length { get { return Max - Min; } }

        // shift a numbers value from one range to the next.
        public double Shift(int num, Range from)
            => RangeHelper.Shift(num, from, this);
    }
}