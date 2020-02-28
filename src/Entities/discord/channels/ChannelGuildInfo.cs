using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class ChannelGuildInfo
    {
        public ChannelGuildInfo(SocketGuildChannel guildChannel)
        {
            Position = guildChannel.Position;
            Overwrites = guildChannel.PermissionOverwrites.ToList();
                
        }

        public int Position { get; }
        public List<Overwrite> Overwrites { get; }
    }
}