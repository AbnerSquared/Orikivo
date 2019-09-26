namespace Orikivo
{
    /// <summary>
    /// A collection of values defining how a result should be marked.
    /// </summary>
    public enum MatchValueHandling
    {
        /// <summary>
        /// Marks data that is equal to the specified value.
        /// </summary>
        Equals = 1,

        /// <summary>
        /// Marks all data that contain a specified value.
        /// </summary>
        Contains = 2,

        /// <summary>
        /// Marks all data that starts with a specified value.
        /// </summary>
        StartsWith = 4,

        /// <summary>
        /// Marks all data that ends with a specified value.
        /// </summary>
        EndsWith = 8
    }
}