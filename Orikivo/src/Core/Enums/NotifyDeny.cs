using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Defines what a <see cref="Notifier"/> ignores.
    /// </summary>
    [Flags]
    public enum NotifyDeny
    {
        /// <summary>
        /// Disables all notifications related to experience.
        /// </summary>
        Level = 1,

        /// <summary>
        /// Disables all notifications related to mail.
        /// </summary>
        Mail = 2,

        /// <summary>
        /// Disables all notifications related to errors.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Disables all notifications related to a <see cref="Desync.Merit"/>.
        /// </summary>
        Merit = 8,

        /// <summary>
        /// Disables all notifications related to travel.
        /// </summary>
        Travel = 16,

        /// <summary>
        /// Disables all notifications values.
        /// </summary>
        All = Level | Mail | Error | Merit | Travel
    }
}
