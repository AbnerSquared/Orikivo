using Discord.WebSocket;
using System.Collections.Generic;

namespace Orikivo
{
    public class ChannelCategoryInfo
    {
        public ChannelCategoryInfo(SocketCategoryChannel categoryChannel)
        {
            List<ChannelBaseInfo> channels = new List<ChannelBaseInfo>();
            foreach (SocketGuildChannel channel in categoryChannel.Channels)
            {
                channels.Add(new ChannelBaseInfo(channel));
            }

            Channels = channels;
        }

        public List<ChannelBaseInfo> Channels { get; }
    }
}