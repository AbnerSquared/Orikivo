using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public static class CalcF
    {
        public const float FLOAT_EPSILON = 1e-3f;
        public const float Pi = 3.14159274f;
        public const float E = 2.71828175f;
        public const float Degree = Pi / 180.0f;

        public static Vector2 PolarToParametric(float magnitude, AngleF direction)
        {
            return new Vector2(magnitude * MathF.Cos(direction.Radians),
                magnitude * MathF.Sin(direction.Radians));
        }

        public static float Hypotenuse(float opposite, float adjacent)
            => MathF.Sqrt((opposite * opposite) + (adjacent * adjacent));

        public static IEnumerable<float> Ceiling(IEnumerable<float> set)
        {
            foreach (float f in set)
                yield return MathF.Ceiling(f);
        }

        public static IEnumerable<float> Floor(IEnumerable<float> set)
        {
            foreach (float f in set)
                yield return MathF.Floor(f);
        }

        public static float Radians(float degrees)
            => degrees * (Pi / 180.0f);

        public static float Degrees(float radians)
            => radians * (180.0f / Pi);

        public static bool MuchGreater(float a, float b, float minDifference = FLOAT_EPSILON)
            => a - minDifference > b;

        public static bool AlmostGreater(float a, float b, float minDifference = FLOAT_EPSILON)
            => a > b - minDifference;

        public static bool AlmostEquals(float a, float b, float minDifference = FLOAT_EPSILON)
            => MathF.Abs(a - b) <= minDifference;

        /// <summary>
        /// Returns the sum of all numbers in a set.
        /// </summary>
        public static float Sum(IEnumerable<float> set)
        {
            float sum = 0;

            foreach (float f in set)
                sum += f;

            return sum;
        }

        /// <summary>
        /// Returns the absolute sum of all numbers in a set.
        /// </summary>
        public static float AbsSum(IEnumerable<float> set)
        {
            float sumAbs = 0;

            foreach (float f in set)
                sumAbs += MathF.Abs(f);

            return sumAbs;
        }

        /// <summary>
        /// Returns the arithmetic mean of a number set.
        /// </summary>
        public static float Average(IEnumerable<float> set)
            => Sum(set) / set.Count();

        /// <summary>
        /// Returns the value that is specified in the middle of a number set.
        /// </summary>
        public static float Median(IEnumerable<float> set)
        {
            int length = set.Count();
            int i = (int)Math.Floor(length / (double) 2);

            // NOTE: x % 2 => Parity, ODD if 1
            return (length % 2) == 0
                ? (set.ElementAt(i - 1) + set.ElementAt(i)) / 2
                : set.ElementAt(i);
        }

        /// <summary>
        /// Interpolates between two values by the specified amount.
        /// </summary>
        public static float Lerp(float a, float b, float amount)
            => a + amount * (b - a);

        /// <summary>
        /// Interpolates between two values using an enforced amount.
        /// </summary>
        public static float LerpExact(float a, float b, float amount)
        {
            if (!RangeF.Contains(0.0f, 1.0f, amount))
                throw new ArithmeticException("Cannot interpolate with an amount outside the range of [0, 1].");

            return (1.0f - amount) * a + amount * b;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float amount)
            => new Vector2(Lerp(a.X, b.X, amount),
                Lerp(a.Y, b.Y, amount));

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

        public static Vector2 Rotate(Vector2 v, AngleF angle)
        {
            if (angle.Degrees == 0)
                return v;

            return Rotate(v.X, v.Y, angle);
        }

        public static IEnumerable<Vector2> RotateAround(Vector2 axis, IEnumerable<Vector2> set, AngleF angle)
        {
            foreach (Vector2 v in set)
                yield return RotateAround(axis, v, angle);
        }

        public static Vector2 RotateAround(Vector2 axis, Vector2 v, AngleF angle)
        {
            if (angle.Degrees == 0)
                return v;

            return RotateAroundAxis(axis.X, axis.Y, v.X, v.Y, angle);
        }

        public static Vector2 RotateAroundAxis(float ax, float ay, float vx, float vy, AngleF angle)
        {
            float x = vx - ax;
            float y = vy - ay;
            Vector2 rotated = Rotate(x, y, angle);

            return new Vector2(ax + rotated.X, ay + rotated.Y);
        }

        public static Vector2 Rotate(float x, float y, AngleF angle)
        {
            return angle.Degrees switch
            {
                0 => new Vector2(x, y),
                90 => Rotate90(x, y),
                180 => Rotate180(x, y),
                270 => Rotate270(x, y),
                _ => new Vector2(
                    x * MathF.Cos(angle.Radians) - (y * MathF.Sin(angle.Radians)),
                    x * MathF.Sin(angle.Radians) + (y * MathF.Cos(angle.Radians)))
            };
        }

        private static Vector2 Rotate90(float x, float y)
            => new Vector2(y, -x);

        private static Vector2 Rotate180(float x, float y)
            => new Vector2(-x, -y);

        private static Vector2 Rotate270(float x, float y)
            => new Vector2(-y, x);

        public static float Distance(Vector2 a, Vector2 b)
            => Distance(a.X, a.Y, b.X, b.Y);

        public static float Distance(float x1, float y1, float x2, float y2)
            => MathF.Sqrt(MathF.Pow(x2 - x1, 2) + MathF.Pow(y2 - y1, 2));

        public static float Slope(Vector2 a, Vector2 b)
            => Slope(a.X, a.Y, b.X, b.Y);

        public static float Slope(float x1, float y1, float x2, float y2)
        {
            float dy = y2 - y1;
            float dx = x2 - x1;
            return dy / dx;
        }

        // TODO: Optimize by simply comparing values instead of using a Dictionary.
        public static float Mode(IEnumerable<float> set)
        {
            var counter = new Dictionary<float, int>();

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
