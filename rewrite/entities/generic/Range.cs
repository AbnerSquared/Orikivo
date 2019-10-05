
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // a range is a class that simplifies defined limits. Min/Max values can be set at once and easily compared to other values.
    public interface IRange<T>
    {
        T Min { get; }
        T Max { get; }
        bool IsEqualTo { get; }
        bool IsInMin(T value);
        bool IsInMax(T value);
        bool ContainsValue(T value);
    }

    public enum RangeSign
    {
        NotEquals = 1, // 1 > 2
        Equals = 2 // 1 >= 2
    }

    public class Range : IRange<int>
    {
        public Range(int max, bool isEqualTo = true)
        {
            Min = 0;
            Max = max;
            IsEqualTo = isEqualTo;
        }
        public Range(int min, int max, bool isEqualTo = true)
        {
            Min = min;
            Max = max;
            IsEqualTo = isEqualTo;
        }

        public int Min { get; set; }
        public int Max { get; set; }
        public bool IsEqualTo { get; set; }

        public bool IsInMin(int value)
            => IsEqualTo ? Min <= value : Min < value;
        public bool IsInMax(int value)
            => IsEqualTo ? Max >= value : Max > value;
        public bool ContainsValue(int value)
         => IsInMin(value) && IsInMax(value);
    }
}
