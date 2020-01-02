namespace Orikivo
{
    /// <summary>
    /// A collection of tags that can be used to group reports.
    /// </summary>
    [System.Flags]
    public enum ReportTag
    {
        /// <summary>
        /// Marks a report as a small error that could be easily fixed.
        /// </summary>
        Minor = 1,

        /// <summary>
        /// Marks a report as a suggestion.
        /// </summary>
        Suggestion = 2,

        /// <summary>
        /// Marks a report as critical, meaning that it could severely harm other processes.
        /// </summary>
        Critical = 3,

        /// <summary>
        /// Marks a report as a visual error (grammatical errors also count).
        /// </summary>
        Visual = 4,

        /// <summary>
        /// Marks a report as derived from an exception.
        /// </summary>
        Exception = 5,

        /// <summary>
        /// Marks a report as automatically generated.
        /// </summary>
        Auto = 6
    }
}
