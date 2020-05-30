using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Gets the <see cref="Regex"/> pattern that is used to parse an <see cref="Emote"/>.
        /// </summary>
        private static readonly string EmoteParsePattern = @"<a?:\w{2,32}:\d{1,20}>";

        /// <summary>
        /// Determines if the specified <see cref="string"/> contains a parseable emote.
        /// </summary>
        private static bool ContainsEmote(string content)
            => new Regex(EmoteParsePattern).Match(content).Success;

        /// <summary>
        /// Returns all captured emotes from a specified <see cref="string"/>.
        /// </summary>
        private static IEnumerable<IEmote> CaptureEmotes(string content)
        {
            List<IEmote> emotes = new List<IEmote>();

            foreach (Match match in new Regex(EmoteParsePattern).Matches(content))
                if (match.Success)
                    emotes.Add(Emote.Parse(match.Value));

            return emotes;
        }

        /// <summary>
        /// Determines if the specified <see cref="IMessage"/> contains any emotes.
        /// </summary>
        public static bool ContainsEmote(this IMessage message)
            => ContainsEmote(message.Content);

        /// <summary>
        /// Returns all emotes that this <see cref="IMessage"/> contains.
        /// </summary>
        public static IEnumerable<IEmote> GetEmotes(this IMessage message)
            => CaptureEmotes(message.Content);

        /// <summary>
        /// Clones this message to the specified <paramref name="channel"/>.
        /// </summary>
        public static async Task<IUserMessage> CloneAsync(
            this IMessage message,
            IMessageChannel channel,
            RequestOptions options = null)
            => await channel.SendMessageAsync(message.Content, message.IsTTS, message.GetRichEmbed()?.Build(), options);

        /// <summary>
        /// Extracts the content from this <paramref name="message"/>.
        /// </summary>
        public static MessageContent Copy(this IMessage message)
        {
            return new MessageContent
            {
                Content = message.Content,
                IsTTS = message.IsTTS,
                Embed = message.GetRichEmbed()
            };
        }
        
        /// <summary>
        /// Attempts to return an <see cref="EmbedType.Rich"/> <see cref="Embed"/> from this <paramref name="message"/>. If none could be found, this return null.
        /// </summary>
        public static EmbedBuilder GetRichEmbed(this IMessage message)
            => message.Embeds
            .FirstOrDefault(x => x.Type == EmbedType.Rich)?
            .ToEmbedBuilder()
            ?? null;
    }
}
