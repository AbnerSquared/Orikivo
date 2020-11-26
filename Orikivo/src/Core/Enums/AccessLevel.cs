namespace Orikivo
{
    /// <summary>
    /// Defines the authority required in order to execute a method.
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>
        /// Inherits the access level from a guild's config.
        /// </summary>
        Inherit = 1,

        /// <summary>
        /// Limits a command to only be utilized by the owner of a guild.
        /// </summary>
        Owner = 2,

        /// <summary>
        /// Limits a command to a developer of Orikivo.
        /// </summary>
        Dev = 3
    }
}
