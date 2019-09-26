using System;

namespace Orikivo
{
    /// <summary>
    /// Defines the basic properties of a cooldown.
    /// </summary>
    public interface ICooldown
    {
        DateTime Execution { get; set; }
        TimeSpan Duration { get; set; }
        bool HasExpired { get; }
        TimeSpan Elapsed { get; }
        TimeSpan Remainder { get; }
    }
}
