namespace Arcadia.Old
{
    /// <summary>
    /// The check method that is used for an attribute criterion.
    /// </summary>
    public enum AttributeMatch
    {
        /// <summary>
        /// If the attribute equals the specified value.
        /// </summary>
        Equals = 1,

        /// <summary>
        /// If the attribute is not equal to the specified value.
        /// </summary>
        NotEquals = 2,

        /// <summary>
        /// If the attribute is greater than the specified value.
        /// </summary>
        Greater = 3,

        /// <summary>
        /// If the attribute is lesser than the specified value.
        /// </summary>
        Lesser = 4,

        /// <summary>
        /// If the attribute is greater than or equal to the specified value.
        /// </summary>
        GreaterOrEquals = 5,

        /// <summary>
        /// If the attribute is lesser than or equal to the specified value.
        /// </summary>
        LesserOrEquals = 6
    }
}
