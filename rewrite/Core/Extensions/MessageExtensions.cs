using Discord;
using System.Collections.Generic;

namespace Orikivo
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Determines if the specified <see cref="IMessage"/> contains any emotes.
        /// </summary>
        public static bool ContainsEmote(this IMessage message)
            => OriRegex.ContainsEmote(message.Content);

        /// <summary>
        /// Returns all emotes that this <see cref="IMessage"/> contains.
        /// </summary>
        public static List<Emote> GetEmotes(this IMessage message)
            => OriRegex.CaptureEmotes(message.Content);
    }
}
