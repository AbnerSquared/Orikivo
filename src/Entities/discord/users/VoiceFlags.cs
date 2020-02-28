namespace Orikivo
{
    public class VoiceFlags
    {
        public VoiceFlags(bool isMuted = false, bool isSelfMuted = false, bool isDeafened = false, bool isSelfDeafened = false, bool isStreaming = false, bool isSuppressed = false)
        {
            IsMuted = isMuted;
            IsSelfMuted = isSelfMuted;
            IsDeafened = isDeafened;
            IsSelfDeafened = isSelfDeafened;
            IsStreaming = isStreaming;
            IsSuppressed = isSuppressed;
        }

        public bool IsMuted { get; }
        public bool IsSelfMuted { get; }
        public bool IsDeafened { get; }
        public bool IsSelfDeafened { get; }
        public bool IsStreaming { get; }
        public bool IsSuppressed { get; }
    }
}