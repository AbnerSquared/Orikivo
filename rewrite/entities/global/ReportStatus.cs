namespace Orikivo
{
    public enum ReportStatus
    {
        Pending = 1, // waiting to be confirmed by others
        Closed = 2, // either complete or declined
        Open = 3 // open to review
    }
}
