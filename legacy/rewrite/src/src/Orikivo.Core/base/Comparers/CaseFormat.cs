namespace Orikivo
{
    /// <summary>
    /// A collection of values in which a string is referred as.
    /// </summary>
    public enum CaseFormat
    {
        /// <summary>
        /// Strings will be computed as their default form.
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// Strings will be computed in lowercase.
        /// </summary>
        Lowercase = 2,

        /// <summary>
        /// Strings will be computed in uppercase.
        /// </summary>
        Uppercase = 4
    }
}