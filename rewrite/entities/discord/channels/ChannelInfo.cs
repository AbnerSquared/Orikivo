using Discord.WebSocket;
using System.Text;

namespace Orikivo
{
    // Add a check to see if it's considered to be the system channel.
    public class ChannelInfo : ChannelBaseInfo
    {
        public ChannelInfo(SocketGuildChannel guildChannel) : base(guildChannel)
        {
            GuildInfo = new ChannelGuildInfo(guildChannel);
        }

        public ChannelInfo(SocketTextChannel textChannel) : this(textChannel as SocketGuildChannel)
        {
            TextInfo = new ChannelTextInfo(textChannel);
        }

        public ChannelInfo(SocketCategoryChannel categoryChannel) : this(categoryChannel as SocketGuildChannel)
        {
            CategoryInfo = new ChannelCategoryInfo(categoryChannel);
        }

        public ChannelInfo(SocketVoiceChannel voiceChannel) : this(voiceChannel as SocketGuildChannel)
        {
            VoiceInfo = new ChannelVoiceInfo(voiceChannel);
        }

        public ChannelTextInfo TextInfo { get; }
        public ChannelVoiceInfo VoiceInfo { get; }
        public ChannelCategoryInfo CategoryInfo { get; }
        public ChannelGuildInfo GuildInfo { get; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            // {channel_name} {channel_id}
            // text
            sb.AppendLine($"**{Name}**#{Id}");
            if (TextInfo != null)
            {
                if (TextInfo.Topic != null)
                    sb.AppendLine(TextInfo.Topic);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}