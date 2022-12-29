using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Orikivo
{
    public abstract class BaseInteractionContext<TContainer, TGuild, TUser> : SocketInteractionContext
        where TGuild : BaseGuild
        where TUser : BaseUser
        where TContainer : BaseContainer<TGuild, TUser>
    {
        protected BaseInteractionContext(DiscordSocketClient client, TContainer data, SocketInteraction interaction)
            : base(client, interaction)
        {
            Data = data;   
        }

        public TContainer Data { get; }

        public TUser Account
        {
            get
            {
                Data.Users.TryGet(User.Id, out TUser user);
                return user;
            }
        }

        public TGuild Server
        {
            get
            {
                if (Guild == null)
                    return null;

                Data.Guilds.TryGet(Guild.Id, out TGuild guild);
                return guild;
            }
            set
            {
                if (Guild == null)
                    return;

                Data.Guilds.AddOrUpdate(Guild.Id, value);
            }
        }

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