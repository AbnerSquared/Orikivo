namespace Orikivo
{
    /// <summary>
    /// Defines an item as sellable.
    /// </summary>
    public interface ISellable
    {
        void Sell();
        ulong Value { get; set; }
    }
}
