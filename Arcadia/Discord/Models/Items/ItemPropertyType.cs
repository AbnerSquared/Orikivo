namespace Arcadia
{
    /// <summary>
    /// Specifies a collection of types for an <see cref="ItemProperty"/>.
    /// </summary>
    public enum ItemPropertyType
    {
        /// <summary>
        /// The property is only referenced within the <see cref="Item"/> itself.
        /// </summary>
        Static = 1,

        /// <summary>
        /// The property is transferred over to the <see cref="Item"/>'s data reference.
        /// </summary>
        Dynamic = 2
    }
}
