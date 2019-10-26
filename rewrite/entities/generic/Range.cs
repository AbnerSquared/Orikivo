namespace Orikivo
{
    public struct Range : IRange<int>
    {
        public Range(int max, bool includeMinMax = true)
        {
            Min = 0;
            Max = max;
            IncludeMinMax = includeMinMax;
        }
        public Range(int min, int max, bool includeMinMax = true)
        {
            Min = min;
            Max = max;
            IncludeMinMax = includeMinMax;
        }

        public int Min { get; set; }
        public int Max { get; set; }
        public bool IncludeMinMax { get; set; }

        public bool IsInMin(int value)
            => IncludeMinMax ? Min <= value : Min < value;
        public bool IsInMax(int value)
            => IncludeMinMax ? Max >= value : Max > value;
        public bool ContainsValue(int value)
            => IsInMin(value) && IsInMax(value);
    }
}
