namespace Orikivo
{
    public enum SchemeRenderingMode
    {
        Static = 0, // stays as a specific color.
        Dynamic = 1, // follows a dynamic scheme, animated.
        Interactive = 2 // aka based on the last command, time of day.
    }
}