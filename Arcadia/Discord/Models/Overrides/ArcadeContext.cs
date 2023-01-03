using Discord;
using Discord.WebSocket;
using Orikivo;

namespace Arcadia
{
    public sealed class ArcadeContext : BaseCommandContext<ArcadeContainer, BaseGuild, ArcadeUser>
    {
        public ArcadeContext(DiscordSocketClient client, ArcadeContainer data, SocketUserMessage message)
            : base(client, data, message)
        {
            if (Guild != null)
            {
                GetOrAddGuild(Guild);
                Server.Synchronize(Guild);
            }
        }

        public override ArcadeUser GetOrAddUser(IUser user)
        {
            if (!Data.Users.TryGet(user.Id, out ArcadeUser value))
            {
                value = new ArcadeUser(user);
                Data.Users.AddOrUpdate(user.Id, value);
            }

            return value;
        }

        public override BaseGuild GetOrAddGuild(IGuild guild)
        {
            if (!Data.Guilds.TryGet(guild.Id, out BaseGuild value))
            {
                value = new BaseGuild(guild);
                Data.Guilds.AddOrUpdate(guild.Id, value);
            }

            return value;
        }
    }
}
