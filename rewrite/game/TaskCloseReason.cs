namespace Orikivo
{
    /// <summary>
    /// Defines what caused a GameTask to close.
    /// </summary>
    public enum TaskCloseReason
    {
        Timeout = 1, // on timeout
        Success = 2, // on success
        Cancel = 3 // on cancel
    }
}
