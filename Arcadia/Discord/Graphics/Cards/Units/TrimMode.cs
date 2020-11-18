namespace Arcadia.Graphics
{
    /// <summary>
    /// Defines how trimming is handled on a card.
    /// </summary>
    public enum TrimMode
    {
        /// <summary>
        /// Trimming is disabled.
        /// </summary>
        None = 1,

        /// <summary>
        /// Trimming is determined by the specified properties.
        /// </summary>
        Optional = 2,

        /// <summary>
        /// Trimming is enabled.
        /// </summary>
        Force = 3
    }
}
