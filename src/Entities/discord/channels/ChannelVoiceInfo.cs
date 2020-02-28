using Discord.WebSocket;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class ChannelVoiceInfo
    {
        public ChannelVoiceInfo(SocketVoiceChannel voiceChannel)
        {
            // figure out how to check if the voice channel is suppressed
            IsSuppressed = false;

            if (voiceChannel.Category != null)
            {
                Category = new ChannelBaseInfo(voiceChannel.Category);
            }

            UserCount = voiceChannel.Users.Count;
            UserLimit = voiceChannel.UserLimit;
            ActiveUsers = voiceChannel.Users.Where(x => x.VoiceState.HasValue).Where(x => x.VoiceState.Value.IsMuted).Count();
            Bitrate = voiceChannel.Bitrate;
        }

        public bool IsSuppressed { get; }
        public ChannelBaseInfo Category { get; }
        public int UserCount { get; }

        public int ActiveUsers { get; }
        public int? UserLimit { get; }
        public int Bitrate { get; }
    }
}