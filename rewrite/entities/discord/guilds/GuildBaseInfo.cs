using Discord.WebSocket;

namespace Orikivo
{
    public class GuildBaseInfo : EntityInfo
    {
        public GuildBaseInfo(SocketGuild guild) : base(guild)
        {
            Owner = new UserBaseInfo(guild.Owner);
        }

        public UserBaseInfo Owner { get; }

    }
}