namespace Orikivo
{
    // a range is a class that simplifies defined limits. Min/Max values can be set at once and easily compared to other values.
    public interface IRange<T> where T : struct
    {
        T Min { get; }
        T Max { get; }
        bool IncludeMinMax { get; }
        bool IsInMin(T value);
        bool IsInMax(T value);
        bool ContainsValue(T value);
    }
}
