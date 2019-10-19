namespace Orikivo
{
    public enum GameLogType
    {
        /// <summary>
        /// The entry is a custom string.
        /// </summary>
        Custom = 1,

        /// <summary>
        ///The entry is an event that was called within the game.
        /// </summary>
        Event = 2,

        /// <summary>
        /// The entry is an event relating to a GameAttribute.
        /// </summary>
        Attribute = 3,

        /// <summary>
        /// The entry derived from an action that occurred within the game.
        /// </summary>
        Action = 4
    }
}
