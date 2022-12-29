using Discord;

namespace Orikivo
{
    public abstract class BaseContext<TContainer, TGuild, TUser>
        where TGuild : BaseGuild
        where TUser : BaseUser
        where TContainer : BaseContainer<TGuild, TUser>
    {
        public TContainer Data { get; }
        public TUser Account { get; }
        public TGuild Server { get; set; }

        public abstract TUser GetOrAddUser(IUser user);

        public abstract TGuild GetOrAddGuild(IGuild guild);

        public void SaveUser(TUser account)
        {
            Data.Users.Save(account);
        }

        public bool TryGetUser(ulong id, out TUser account)
            => Data.Users.TryGet(id, out account);

        public bool TryGetGuild(ulong id, out TGuild server)
            => Data.Guilds.TryGet(id, out server);

        /// <summary>
        /// Returns the default prefix for the specified command context.
        /// </summary>
        public string GetPrefix()
            => Account?.Config.Prefix ?? Server?.Config.Prefix ?? Constants.DEFAULT_PREFIX;
    }
}
