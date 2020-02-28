using Discord;
using Discord.WebSocket;
using System;
using System.Linq;

namespace Orikivo
{
    // base is generic, doesn't specify much
    public class ChannelBaseInfo : EntityInfo
    {
        public ChannelBaseInfo(IChannel channel) : base(channel)
        {
            Type = ChannelType.Unknown;
        }

        public ChannelBaseInfo(SocketGuildChannel channel) : base(channel)
        {
            Type = ChannelType.Unknown;
            if (channel.Guild.CategoryChannels.Any(x => x.Id == channel.Id))
                Type = ChannelType.Category;
            else if (channel.Guild.TextChannels.Any(x => x.Id == channel.Id))
                Type = ChannelType.Text;
            else if (channel.Guild.VoiceChannels.Any(x => x.Id == channel.Id))
                Type = ChannelType.Voice;
        }

        public ChannelBaseInfo(SocketDMChannel privateChannel) : base(privateChannel)
        {
            Type = ChannelType.Private;
        }

        public ChannelBaseInfo(SocketGroupChannel groupChannel) : base(groupChannel)
        {
            Type = ChannelType.Group;
        }

        public ChannelType Type { get; }
    }
}