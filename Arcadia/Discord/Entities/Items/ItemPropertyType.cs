namespace Arcadia
{
    public enum ItemPropertyType
    {
        /// <summary>
        /// The property is only referenced within the item itself.
        /// </summary>
        Static = 1,

        /// <summary>
        /// The property is transferred over to the item's data reference.
        /// </summary>
        Dynamic = 2
    }
}
