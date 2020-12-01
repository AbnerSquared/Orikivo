namespace Orikivo
{
    /// <summary>
    /// Represents a generic data container.
    /// </summary>
    /// <typeparam name="TGuild"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public class BaseContainer<TGuild, TUser>
        where TGuild : BaseGuild
        where TUser : BaseUser
    {
        public BaseContainer()
        {
            Users = new JsonContainer<TUser>(@"..\data\users\");
            Guilds = new JsonContainer<TGuild>(@"..\data\guilds\");
        }

        public JsonContainer<TUser> Users { get; }

        public JsonContainer<TGuild> Guilds { get; }
    }
}
