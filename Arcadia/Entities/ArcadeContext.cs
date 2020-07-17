using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Desync;
using Orikivo.Framework;

namespace Arcadia
{
    public class ArcadeContext : SocketCommandContext
    {
        public ArcadeContext(DiscordSocketClient client, ArcadeContainer data, SocketUserMessage message)
            : base(client, message)
        {
            Logger.Debug("-- Intializing arcade context. --");
            Data = data;
            
            if (Guild != null)
            {
                GetOrAddGuild(Guild);
                Server.Synchronize(Guild);
            }

            Logger.Debug($"-- {(Account == null ? "Account exists" : "Account does not exist")}. --");
        }

        internal ArcadeContainer Data { get; }

        public ArcadeUser Account
        {
            get
            {
                Data.Users.TryGetValue(User.Id, out ArcadeUser user);
                return user;
            }
        }

        public BaseGuild Server
        {
            get
            {
                Data.Guilds.TryGetValue(Guild.Id, out BaseGuild guild);
                return guild;
            }
            set => Data.Guilds.AddOrUpdate(Guild.Id, value);
        }

        internal ArcadeUser GetOrAddUser(SocketUser user)
        {
            if (!Data.Users.TryGetValue(user.Id, out ArcadeUser value))
            {
                value = new ArcadeUser(user);
                Data.Users.AddOrUpdate(user.Id, value);
            }

            return value;
        }
        internal BaseGuild GetOrAddGuild(SocketGuild guild)
        {
            if (!Data.Guilds.TryGetValue(guild.Id, out BaseGuild value))
            {
                value = new BaseGuild(guild);
                Data.Guilds.AddOrUpdate(guild.Id, value);
            }

            return value;
        }

        public bool TryGetUser(ulong id, out ArcadeUser account)
            => Data.Users.TryGetValue(id, out account);

        public bool TryGetGuild(ulong id, out BaseGuild server)
            => Data.Guilds.TryGetValue(id, out server);
    }
}
