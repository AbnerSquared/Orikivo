namespace Orikivo
{
    /// <summary>
    /// Defines how the item is locked, alongside what is required for the item to unlock.
    /// </summary>
    public class ItemLockInfo
    {
        /// <summary>
        /// The criteria that the user needs to meet in order to unlock the item.
        /// </summary>
        public UserCriteria ToUnlock { get; }
    }
}
