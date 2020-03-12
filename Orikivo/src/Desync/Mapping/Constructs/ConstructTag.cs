namespace Orikivo.Desync
{
    /// <summary>
    /// Defines the specific type of a <see cref="Construct"/>.
    /// </summary>
    [System.Flags]
    public enum ConstructType
    {
        /// <summary>
        /// Defines the <see cref="Construct"/> as a <see cref="Desync.Highrise"/>.
        /// </summary>
        Highrise = 1,

        /// <summary>
        /// Defines the <see cref="Construct"/> as a <see cref="Desync.Market"/>.
        /// </summary>
        Market = 2,

        /// <summary>
        /// Defines the <see cref="Construct"/> as a <see cref="Desync.Casino"/>.
        /// </summary>
        Casino = 4
    }

}
