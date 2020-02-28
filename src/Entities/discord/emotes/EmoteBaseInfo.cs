using Discord;

namespace Orikivo
{
    public class EmoteBaseInfo : EntityInfo
    {
        public EmoteBaseInfo(Emote emote) : base(emote)
        {
            Url = emote.Url;
            IsAnimated = emote.Animated;
        }

        public string Url { get; }
        public bool IsAnimated { get; }
    }
}