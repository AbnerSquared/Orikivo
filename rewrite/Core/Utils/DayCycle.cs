namespace Orikivo
{
    /// <summary>
    /// Represents a specific cycle during a day.
    /// </summary>
    public enum DayCycle
    {
        Dawn = 1,
        Sunrise = 2,

        /// <summary>
        /// Represents a mid-day cycle.
        /// </summary>
        Meridian = 3,
        Sunset = 4,
        Dusk = 5,
        Night = 6
    }
}
