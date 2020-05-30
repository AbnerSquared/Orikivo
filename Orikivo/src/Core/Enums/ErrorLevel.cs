namespace Orikivo
{
    /// <summary>
    /// Defines how runtime errors are handled.
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>
        /// All errors will be disabled (not recommended).
        /// </summary>
        Quiet = 1,

        /// <summary>
        /// All runtime errors are shown, regardless of its severity.
        /// </summary>
        Verbose = 2,

        /// <summary>
        /// Errors are only shown when an exception occurs during execution.
        /// </summary>
        Critical = 3
    }
}
