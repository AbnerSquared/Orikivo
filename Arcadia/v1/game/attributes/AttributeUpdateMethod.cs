namespace Arcadia.Old
{
    /// <summary>
    /// The method that is used when updating an attribute.
    /// </summary>
    public enum AttributeUpdateMethod
    {
        /// <summary>
        /// Specifies to add onto the attribute. (a += b)
        /// </summary>
        Add = 1,

        /// <summary>
        /// Specifies to set the attribute. (a = b)
        /// </summary>
        Set = 2, // Clear can be set within GameProperties.

        /// <summary>
        /// Specifies to remove from the attribute. (a -= b)
        /// </summary>
        Remove = 3

        // Multiply
        // Divide
    }
}
