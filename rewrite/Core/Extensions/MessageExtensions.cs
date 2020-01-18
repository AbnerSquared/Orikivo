using Discord;
using System.Collections.Generic;

namespace Orikivo
{
    public static class MessageExtensions
    {
        public static bool ContainsEmote(this IMessage message)
            => OriRegex.ContainsEmote(message.Content);

        public static List<Emote> GetEmotes(this IMessage message)
        {
            OriRegex.CaptureEmotes(message.Content, out List<Emote> emotes);
            return emotes;
        }
    }
}
