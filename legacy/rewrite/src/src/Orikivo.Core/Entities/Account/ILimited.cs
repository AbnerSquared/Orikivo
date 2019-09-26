namespace Orikivo
{
    /// <summary>
    /// Defines an item as limited.
    /// </summary>
    public interface ILimited
    {
        /// <summary>
        /// The most of this object you can hold at once.
        /// </summary>
        int Max { get; set; }
    }
}
