using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Orikivo
{
    public class ChannelTextInfo
    {
        public ChannelTextInfo(SocketTextChannel textChannel)
        {
            Topic = textChannel.Topic;

            Flags = new ChannelFlags(textChannel.IsNsfw);

            if (textChannel.Category != null)
            {
                Category = new ChannelBaseInfo(textChannel.Category);
            }
            UserCount = textChannel.Users.Count;
            SlowModeInterval = textChannel.SlowModeInterval;
        }

        public string Topic { get; }
        public ChannelFlags Flags { get; }
        public ChannelBaseInfo Category { get; }

        public int SlowModeInterval { get; }
        public int UserCount { get; }
    }
}