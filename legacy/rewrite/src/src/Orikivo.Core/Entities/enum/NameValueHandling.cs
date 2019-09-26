namespace Orikivo
{
    public enum NameFormat
    {
        /// <summary>
        /// The name that the user has currently set will be the value displayed.
        /// </summary>
        Default = 1,

        /// <summary>
        /// All nicknames are ignored, only getting their pure username.
        /// </summary>
        Username = 2,

        /// <summary>
        /// All users are referred by their mentionable name. (eg. Name#Discriminator)
        /// </summary>
        Direct = 4, 

        /// <summary>
        /// All users are known by their id.
        /// </summary>
        Id = 8
    }
}
