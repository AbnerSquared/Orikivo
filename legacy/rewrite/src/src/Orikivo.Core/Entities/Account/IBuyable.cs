namespace Orikivo
{
    /// <summary>
    /// Defines an item as purchasable.
    /// </summary>
    public interface IBuyable
    {
        void Buy();
        ulong Cost { get; set; }
    }
}
