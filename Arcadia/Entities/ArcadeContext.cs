using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo;

namespace Arcadia
{
    public class ArcadeContext : SocketCommandContext
    {
        public ArcadeContext(DiscordSocketClient client, ArcadeContainer data, SocketUserMessage message)
            : base(client, message)
        {
            Data = data;

            if (Guild != null)
            {
                GetOrAddGuild(Guild);
                Server.Synchronize(Guild);
            }
        }

        internal ArcadeContainer Data { get; }

        public ArcadeUser Account
        {
            get
            {
                Data.Users.TryGet(User.Id, out ArcadeUser user);
                return user;
            }
        }

        public BaseGuild Server
        {
            get
            {
                if (Guild == null)
                    return null;

                Data.Guilds.TryGet(Guild.Id, out BaseGuild guild);
                return guild;
            }
            set
            {
                if (Guild == null)
                    return;

                Data.Guilds.AddOrUpdate(Guild.Id, value);
            }
        }

        internal ArcadeUser GetOrAddUser(IUser user)
        {
            if (!Data.Users.TryGet(user.Id, out ArcadeUser value))
            {
                value = new ArcadeUser(user);
                Data.Users.AddOrUpdate(user.Id, value);
            }

            return value;
        }
        internal BaseGuild GetOrAddGuild(IGuild guild)
        {
            if (!Data.Guilds.TryGet(guild.Id, out BaseGuild value))
            {
                value = new BaseGuild(guild);
                Data.Guilds.AddOrUpdate(guild.Id, value);
            }

            return value;
        }

        internal void SaveUser(ArcadeUser account)
        {
            Data.Users.Save(account);
        }

        public bool TryGetUser(ulong id, out ArcadeUser account)
            => Data.Users.TryGet(id, out account);

        public bool TryGetGuild(ulong id, out BaseGuild server)
            => Data.Guilds.TryGet(id, out server);
    }
}
