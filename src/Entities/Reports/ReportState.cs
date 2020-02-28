namespace Orikivo
{
    /// <summary>
    /// Defines the state of a report.
    /// </summary>
    public enum ReportState
    {
        /// <summary>
        /// Marks a report as open to review.
        /// </summary>
        Open = 1,

        /// <summary>
        /// States that a report has been fixed or declined.
        /// </summary>
        Closed = 2
    }
}
