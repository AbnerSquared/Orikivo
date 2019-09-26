namespace Orikivo
{
    /// <summary>
    /// Defines the types that an viewpoint of an object can be.
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// The object can be publicly viewed by anyone.
        /// </summary>
        Public = 1,

        /// <summary>
        /// The object can only be viewed inside the server it's executed in.
        /// </summary>
        Unlisted = 2,

        /// <summary>
        /// The object can only be viewed through direct messages or a private channel.
        /// </summary>
        Private = 3
    }
}
