namespace Orikivo
{
    public class CachedVendorInteraction
    {
        public ulong UserId {get; private set;} // the user that they have interacted with.
        public double Relations {get; private set;} // the status of their relations with the vendor. -1 = bad, 1 = good.
    }
}