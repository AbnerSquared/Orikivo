using System;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Flags that represents the various types of notifiers that can be ignored.
    /// </summary>
    [Flags]
    public enum NotifyDeny
    {
        /// <summary>
        /// Disables notifiers related to experience.
        /// </summary>
        Level = 1,

        /// <summary>
        /// Disables notifiers related to mail.
        /// </summary>
        Mail = 2,
        /// <summary>
        /// Disables notifiers related to errors.
        /// </summary>
        Error = 4,
        /// <summary>
        /// Disables notifiers related to a <see cref="Orikivo.Unstable.Merit"/>.
        /// </summary>
        Merit = 8,
        /// <summary>
        /// All notifiers are disabled.
        /// </summary>
        All = Level | Mail | Error | Merit
    }
}
