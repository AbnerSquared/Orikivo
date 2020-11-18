namespace Arcadia.Graphics
{
    /// <summary>
    /// Defines how component offsets are handled.
    /// </summary>
    public enum OffsetHandling
    {
        // The specified Offset will replace the cursor
        /// <summary>
        /// Offsets will replace the existing cursor offset.
        /// </summary>
        Replace = 1,

        /// <summary>
        /// Offsets will be added to the existing cursor offset.
        /// </summary>
        Additive = 2
    }
}
