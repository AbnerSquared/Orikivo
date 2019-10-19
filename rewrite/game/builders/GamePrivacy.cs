namespace Orikivo
{
    /// <summary>
    /// Defines the visibility of a game.
    /// </summary>
    public enum GamePrivacy
    {
        /// <summary>
        /// This permits anyone within a shard to join a game.
        /// </summary>
        Public = 1,

        /// <summary>
        /// This permits anyone within the guild the game was created in to join.
        /// </summary>
        Local = 2
    }
}
