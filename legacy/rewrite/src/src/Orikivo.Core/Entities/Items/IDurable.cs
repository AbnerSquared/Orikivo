namespace Orikivo
{
    /// <summary>
    /// Defines an item as durable.
    /// </summary>
    public interface IDurable
    {
        ulong Durability {get; set;}
    }
}