using Discord.WebSocket;

namespace Orikivo
{
    public class UserVoiceInfo
    {
        public UserVoiceInfo(SocketVoiceState voice)
        {
            Flags = new VoiceFlags(voice.IsMuted, voice.IsSelfMuted, voice.IsDeafened, voice.IsSelfDeafened, voice.IsStreaming, voice.IsSuppressed);
            SessionId = voice.VoiceSessionId;
            Channel = new ChannelBaseInfo(voice.VoiceChannel);

        }

        public string SessionId { get; }
        public VoiceFlags Flags { get; }
        public ChannelBaseInfo Channel { get; }
    }
}