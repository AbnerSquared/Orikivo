using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents how errors are handled.
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>
        /// All errors will be disabled (not recommended).
        /// </summary>
        Quiet = 1,

        /// <summary>
        /// All errors are shown.
        /// </summary>
        Verbose = 2,

        /// <summary>
        /// Errors are only shown when an <see cref="Exception"/> occurs during execution.
        /// </summary>
        Critical = 3
    }
}
