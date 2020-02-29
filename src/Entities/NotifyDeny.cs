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
        /// Disables the <see cref="Notification"/> values related to experience.
        /// </summary>
        Level = 1,

        /// <summary>
        /// Disables the <see cref="Notification"/> values related to mail.
        /// </summary>
        Mail = 2,

        /// <summary>
        /// Disables the <see cref="Notification"/> values related to errors.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Disables the <see cref="Notification"/> values related to a <see cref="Desync.Merit"/>.
        /// </summary>
        Merit = 8,

        /// <summary>
        /// Disables the <see cref="Notification"/> values related to travel.
        /// </summary>
        Travel = 16,

        /// <summary>
        /// Disables all <see cref="Notification"/> values.
        /// </summary>
        All = Level | Mail | Error | Merit | Travel
    }
}
