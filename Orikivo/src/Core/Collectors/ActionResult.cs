namespace Orikivo
{
    /// <summary>
    /// Defines what the <see cref="MessageCollector"/> continues with once an action is complete.
    /// </summary>
    public enum ActionResult
    {
        /// <summary>
        /// Closes the <see cref="MessageCollector"/> and marks the proceeding action as a success.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Closes the <see cref="MessageCollector"/> and marks the proceeding action as a failure.
        /// </summary>
        Fail = 2,

        /// <summary>
        /// Allows the <see cref="MessageCollector"/> to continue processing messages.
        /// </summary>
        Continue = 3
    }
}
