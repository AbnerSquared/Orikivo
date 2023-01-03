namespace Arcadia
{
    public enum TextOverflowMode
    {
        Clip = 1, // All text over the limit is cut
        Error = 2 // If text is longer than the specified limit, throw an error
    }
}
