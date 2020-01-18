namespace Orikivo
{
    public enum AccessLevel
    {
        /// <summary>
        /// Inherit's the access level from a <see cref="OriGuild"/>'s config.
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
