using System;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Flags that represents the various types of notifiers that can be ignored.
    /// </summary>
    [Flags]
    public enum NotifyDeny
    {
        Level = 1,
        Mail = 2,
        Error = 4,
        Merit = 8,
        All = Level | Mail | Error | Merit
    }
}
