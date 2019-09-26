namespace Orikivo
{
    /// <summary>
    /// Defines an item as tradable.
    /// </summary>
    public interface ITradable
    {
        void Trade();
        int TradesLeft { get; set; }
    }
}
