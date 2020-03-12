namespace Orikivo.Drawing
{
    public struct Range
    {
        public static bool Contains(long min, long max, long value, bool inclusiveMin = true, bool inclusiveMax = true)
            => (inclusiveMin ? value >= min : value > min)
            && (inclusiveMax ? value <= max : value < max);
    }
}
