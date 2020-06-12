namespace Arcadia.Old
{
    /// <summary>
    /// Defines what caused a GameTask to close.
    /// </summary>
    public enum TaskQueueReason
    {
        Timeout = 1, // on timeout
        Success = 2, // on success
        Cancel = 3 // on cancel
    }
}
