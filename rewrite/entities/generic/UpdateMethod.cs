namespace Orikivo
{
    /// <summary>
    /// Defines the generic methods that can be used for attributes derived from Int32.
    /// </summary>
    public enum UpdateMethod
    {
        /// <summary>
        /// Specifies to add onto a variable. (a += b)
        /// </summary>
        Add = 1,

        /// <summary>
        /// Specifies to set the variable. (a = b)
        /// </summary>
        Set = 2,

        /// <summary>
        /// Specifies to remove from a variable. (a -= b)
        /// </summary>
        Remove = 3
    }
}
