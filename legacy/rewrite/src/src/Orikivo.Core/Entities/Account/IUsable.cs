using System;

namespace Orikivo
{
    /// <summary>
    /// Defines an item as usable.
    /// </summary>
    public interface IUsable
    {
        ulong Power { get; set; }
        TimeSpan? Duration { get; set; }
        //UsageType UsageMode { get; set; }
        ActionType Action { get; set; }
        int Uses { get; set; }
    }
}
