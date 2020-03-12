namespace Orikivo.Desync
{
    /// <summary>
    /// Defines the specific type of a <see cref="Structure"/>.
    /// </summary>
    [System.Flags]
    public enum StructureType
    {
        /// <summary>
        /// Defines the <see cref="Structure"/> as a portable safe zone.
        /// </summary>
        Tent = 1,

        /// <summary>
        /// Defines the <see cref="Structure"/> as a previous desynchronization marker.
        /// </summary>
        Memorial = 2,
        
        /// <summary>
        /// Defines the <see cref="Structure"/> as a simple vanity.
        /// </summary>
        Decoration = 3
    }
}
