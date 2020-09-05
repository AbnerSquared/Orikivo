namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a relationship level between two characters.
    /// </summary>
    public enum AffinityLevel
    {
        /// <summary>
        /// Marks the relationship as people who hate each other to an extreme degree.
        /// </summary>
        Enemy = -3,

        /// <summary>
        /// Marks the relationship as people who hate each other.
        /// </summary>
        Hatred = -2,


        /// <summary>
        /// Marks the relationship as people who dislike each other.
        /// </summary>
        Annoyed = -1,

        /// <summary>
        /// Marks the relationship as completely new.
        /// </summary>
        Stranger = 0,
        
        /// <summary>
        /// Marks the relationship as people that have met.
        /// </summary>
        Familiar = 1,

        /// <summary>
        /// Marks the relationship as people who know each other.
        /// </summary>
        Friend = 2,

        /// <summary>
        /// Marks the relationship as people who know each other very well.
        /// </summary>
        BestFriend = 3
    }
}
