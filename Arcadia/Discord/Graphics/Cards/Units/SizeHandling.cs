namespace Arcadia.Graphics
{
    /// <summary>
    /// Defines how component size specifications are handled.
    /// </summary>
    public enum SizeHandling
    {
        /// <summary>
        /// Size limitations are ignored.
        /// </summary>
        Ignore = 0,

        /// <summary>
        /// Size limitations will throw an exception if the reference size is larger.
        /// </summary>
        Throw = 1,

        /// <summary>
        /// Size limitations will override the reference size.
        /// </summary>
        Set = 2,

        /// <summary>
        /// Size limitations will represent the clipping bounds if the reference size is larger.
        /// </summary>
        Clip = 3
    }
}
