namespace Orikivo.Desync
{
    /// <summary>
    /// Defines how a <see cref="Barrier"/> blocks a <see cref="Husk"/>.
    /// </summary>
    [System.Flags]
    public enum BarrierTag
    {
        /// <summary>
        /// Defines the <see cref="Barrier"/> as a physical roadblock.
        /// </summary>
        Physical = 1,

        /// <summary>
        /// Defines the <see cref="Barrier"/> as a visual roadblock.
        /// </summary>
        Visual = 2,

        All = Physical | Visual
    }
}
