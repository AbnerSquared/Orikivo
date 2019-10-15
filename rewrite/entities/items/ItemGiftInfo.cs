namespace Orikivo
{
    /// <summary>
    /// Defines how an item is handled when gifting.
    /// </summary>
    public class ItemGiftInfo
    {
        /// <summary>
        /// The amount of times an item can be gifted until it gift-locks. If left empty, it will default to no limit.
        /// </summary>
        public int? MaxGifts { get; } = null;
    }
}
