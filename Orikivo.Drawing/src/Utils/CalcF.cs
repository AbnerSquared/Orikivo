using System;
using System.Collections.Generic;
using System.Linq;
using MathF = System.MathF;

namespace Orikivo.Drawing
{

    public static class CalcF
    {
        // AlmostEquals(float a, float b, float difference)
        // AlmostAllEquals(float a, float b, int maxMismatches)
        // MuchGreater
        // MuchLesser

        public const float FLOAT_EPSILON = 1e-3f;
        public const float Pi = 3.14159274f;
        public const float E = 2.71828175f;
        public const float Degree = Pi / 180.0f;

        public static float Hypotenuse(float opposite, float adjacent)
            => MathF.Sqrt((opposite * opposite) + (adjacent * adjacent));

        public static IEnumerable<float> Ceiling(IEnumerable<float> set)
        {
            List<float> ceilingSet = new List<float>();

            foreach (float f in set)
                ceilingSet.Add(MathF.Ceiling(f));

            return ceilingSet;
        }

        public static IEnumerable<float> Floor(IEnumerable<float> set)
        {
            List<float> floorSet = new List<float>();

            foreach (float f in set)
                floorSet.Add(MathF.Floor(f));

            return floorSet;
        }

        public static float Radians(float degrees)
        {
            return degrees * (Pi / 180.0f);
        }

        public static float Degrees(float radians)
        {
            return radians * (180.0f / Pi);
        }

        public static bool MuchGreater(float a, float b, float minDifference = FLOAT_EPSILON)
        {
            return a - minDifference > b;
        }

        public static bool AlmostGreater(float a, float b, float minDifference = FLOAT_EPSILON)
        {
            return a > b - minDifference;
        }

        public static bool AlmostEquals(float a, float b, float minDifference = FLOAT_EPSILON)
        {
            return MathF.Abs(a - b) <= minDifference;
        }

        public static float Sum(IEnumerable<float> set)
        {
            float sum = 0;

            foreach (float f in set)
                sum += f;

            return sum;
        }

        /// <summary>
        /// Returns the arithmetic mean of the specified number set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static float Average(IEnumerable<float> set)
        {
            return Sum(set) / set.Count();
        }

        /// <summary>
        /// Returns the <see cref="float"/> that is positioned in the middle of the specified number set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static float Median(IEnumerable<float> set)
        {
            int length = set.Count();
            int i = (int)Math.Floor((double)(length / 2));
            if (Calc.Parity(length) == 0) // if it's even
            {
                
                // when it's odd, you get the average of the two center values
                return (set.ElementAt(i - 1) + set.ElementAt(i)) / 2;
            }
            else
            {
                return set.ElementAt(i);
            }

            // likewise if its odd, you can just return the value at the middle.
        }

        /// <summary>
        /// Interpolates between two values by the specified amount.
        /// </summary>
        public static float Lerp(float a, float b, float amount)
        {
            return  a + (amount * (b - a));
        }

        /// <summary>
        /// Interpolates between two values using an enforced amount.
        /// </summary>
        public static float LerpExact(float a, float b, float amount)
        {
            if (!RangeF.Contains(0.0f, 1.0f, amount))
                throw new ArithmeticException("Cannot interpolate with an amount outside the range of [0, 1].");

            return (1.0f - amount) * a + amount * b;
        }
        
        // linear interpolation
        public static Vector2 Lerp(Vector2 a, Vector2 b, float amount)
        {
            return new Vector2(Lerp(a.X, b.X, amount), Lerp(a.Y, b.Y, amount));
        }

        /// <summary>
        /// Returns the smallest <see cref="float"/> value from a specified collection.
        /// </summary>
        public static float Min(float a, float b, params float[] rest)
        {
            float min = MathF.Min(a, b);

            foreach (float f in rest)
                min = MathF.Min(min, f);

            return min;
        }

        /// <summary>
        /// Returns the largest <see cref="float"/> value from a specified collection.
        /// </summary>
        public static float Max(float a, float b, params float[] rest)
        {
            float max = MathF.Max(a, b);

            foreach (float f in rest)
                max = MathF.Max(max, f);

            return max;
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            float dist = MathF.Sqrt(MathF.Pow(b.X - a.X, 2) + MathF.Pow(b.Y - a.Y, 2));

            return dist;
        }

        public static float Slope(Vector2 a, Vector2 b)
        {
            float dy = b.Y - a.Y;
            float dx = b.X - a.X;

            float m = dy / dx;

            return m;
        }

        // TODO: Optimize by simply comparing values instead of using a Dictionary.
        public static float Mode(IEnumerable<float> set)
        {
            Dictionary<float, int> counter = new Dictionary<float, int>();

            foreach (float number in set)
            {
                if (!counter.TryAdd(number, 1))
                    counter[number]++;
            }

            return counter
                .GroupBy(x => x.Value)
                .OrderByDescending(x => x.Key).First()
                .Select(x => x.Key).First();
        }
    }
}
