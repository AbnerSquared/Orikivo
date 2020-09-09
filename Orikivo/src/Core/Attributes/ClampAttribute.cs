using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ClampAttribute : Attribute
    {
        public ClampAttribute(int max)
        {
            Max = max;
            HasMin = false;
        }

        public ClampAttribute(int min, int max)
        {
            if (min >= max)
                throw new ArgumentException("The lower bound cannot be larger or equal to the upper bound.");

            Min = min;
            Max = max;
            HasMin = true;
        }

        public bool HasMin { get; }
        public int Min { get; }
        public int Max { get; }
    }
}
