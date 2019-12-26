using Discord;

namespace Orikivo
{
    public class CardDetails
    {
        public string Name;
        public string AvatarUrl;
        public ulong Balance;
        public ulong Exp;
        public ulong NextLevelExp;
        public ulong CurrentLevelExp;
        public int Level;
        public int Ascent;
        public string Activity;
        public UserStatus Status;
    }

    public enum CardImageSize
    {
        Small = 16,
        Large = 32
    }
}
