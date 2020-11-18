namespace Arcadia.Graphics
{
    /// <summary>
    /// Defines how component offsets are managed post-operation.
    /// </summary>
    public enum OffsetFinalize
    {
        /// <summary>
        /// Offset modifications will be reverted.
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// Offset modifications will remain.
        /// </summary>
        Keep = 2
    }
}
