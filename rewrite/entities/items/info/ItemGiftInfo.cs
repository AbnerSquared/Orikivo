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
        public int? MaxGifts { get; internal set; } = null;

        /// <summary>
        /// Determines if the user that this is being gifted to requires meeting the ToOwn criteria set.
        /// </summary>
        public bool RequireOwnCriteria { get; internal set; }
    }
}
