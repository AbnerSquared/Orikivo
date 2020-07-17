using System;

namespace Orikivo.Drawing
{
    public struct RangeF
    {
        public static readonly RangeF Percent = new RangeF(0.00f, 1.00f);
        public static readonly RangeF Normal = new RangeF(-1.00f, 1.00f);
        public static readonly RangeF Degree = new RangeF(0.00f, 360.00f, true, false);

        public static float Convert(RangeF from, RangeF to, float value)
            => Convert(from.Min, from.Max, to.Min, to.Max, value);

        public static float Convert(float fromMin, float fromMax, float toMin, float toMax, float value)
        {
            float from = fromMax - fromMin;
            float to = toMax - toMin;

            if (from == 0)
                return toMin;

            return ((value - fromMin) * to / from) + toMin;
        }

        /// <summary>
        /// Flattens the <see cref="RangeF"/> to whole numbers.
        /// </summary>
        public static RangeF Truncate(RangeF range)
            => new RangeF(MathF.Truncate(range.Min),
                MathF.Truncate(range.Max),
                range.InclusiveMin,
                range.InclusiveMax);

        public static float Clamp(RangeF range, float value)
            => Clamp(range.Min, range.Max, value);

        public static float Clamp(float min, float max, float value)
            => value > max
            ? max
            : value < min
            ? min
            : value;

        public static bool Contains(float min, float max, float value, bool inclusiveMin = true, bool inclusiveMax = true)
            => (inclusiveMin ? value >= min : value > min)
            && (inclusiveMax ? value <= max : value < max);

        public RangeF(float max, bool inclusiveMin = true, bool inclusiveMax = true)
        {
            Min = 0f;
            Max = max;
            InclusiveMin = inclusiveMin;
            InclusiveMax = inclusiveMax;
        }

        public RangeF(float min, float max, bool inclusiveMin = true, bool inclusiveMax = true)
        {
            Min = min;
            Max = max;
            InclusiveMin = inclusiveMin;
            InclusiveMax = inclusiveMax;
        }

        /// <summary>
        /// Gets the lower bound of the current <see cref="RangeF"/>.
        /// </summary>
        public float LowerBound => InclusiveMin ? Min : (Min - float.Epsilon);

        /// <summary>
        /// Gets the upper bound of the current <see cref="RangeF"/>.
        /// </summary>
        public float UpperBound => InclusiveMax ? Max : (Max - float.Epsilon);

        public float Min { get; }

        public float Max { get; }

        public bool InclusiveMin { get; }

        public bool InclusiveMax { get; }

        public float Length => MathF.Abs(UpperBound - LowerBound);

        public float Clamp(float value)
            => Clamp(Min, Max, value);

        public float Convert(RangeF range, float value)
            => Convert(range.Min, range.Max, Min, Max, value);

        public float Convert(float min, float max, float value)
            => Convert(min, max, Min, Max, value);

        public bool All(params float[] values)
        {
            foreach (float value in values)
                if (!Contains(value))
                    return false;

            return true;
        }

        public bool Contains(float value)
            => Contains(Min, Max, value, InclusiveMin, InclusiveMax);
    }
}
