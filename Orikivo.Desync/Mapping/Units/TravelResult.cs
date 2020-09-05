namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the result of a travel.
    /// </summary>
    public enum TravelResult
    {
        Start = 1, // if the place does exist, but requires travel time to reach it.
        Instant = 2, // if the place does exist and can be travelled to instantly.
        Closed = 3, // if the place does exist, but cannot be accessed right now.
        Invalid = 4 // if the place doesn't exist.
    }
}
