namespace Arcadia
{
    /// <summary>
    /// Represents a collection of cooldown variations for an <see cref="Arcadia.Item"/> that is used.
    /// </summary>
    public enum CooldownMode
    {
        /// <summary>
        /// Cooldowns are applied to the unique instance of an item.
        /// </summary>
        Instance = 1,

        /// <summary>
        /// Cooldowns are applied to the item itself.
        /// </summary>
        Item = 2,

        // NOTE: Throw an exception if no group is specified
        /// <summary>
        /// Cooldowns are applied to the group of an item.
        /// </summary>
        Group = 3,

        /// <summary>
        /// Cooldowns are applied to all items.
        /// </summary>
        Global = 4
    }
}
