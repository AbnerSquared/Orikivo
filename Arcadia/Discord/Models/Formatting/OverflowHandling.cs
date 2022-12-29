namespace Arcadia
{
    public enum OverflowHandling
    {
        Clip = 1, // All text over the limit is cut
        Error = 2 // If text is longer than the specified limit, throw an error
    }
}
