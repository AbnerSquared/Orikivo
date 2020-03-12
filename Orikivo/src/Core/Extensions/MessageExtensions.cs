using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    public class MessageContent
    {
        public string Content { get; set; }
        
        public bool IsTTS { get; set; }

        public EmbedBuilder Embed { get; set; }
    }

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

        /// <summary>
        /// Clones this message to the specified <paramref name="channel"/>.
        /// </summary>
        public static async Task<IUserMessage> CloneAsync(this IMessage message, IMessageChannel channel, RequestOptions options = null)
            => await channel.SendMessageAsync(message.Content, message.IsTTS, message.GetRichEmbed(), options);

        // extracts the content from the specified message.
        public static MessageContent Copy(this IMessage message)
        {
            return new MessageContent
            {
                Content = message.Content,
                IsTTS = message.IsTTS,
                Embed = message.GetRichEmbed()?.ToEmbedBuilder()
            };
        }
        
        // attempts to get a rich embed from the specified message; otherwise, null.
        public static Embed GetRichEmbed(this IMessage message)
            => message.Embeds.FirstOrDefault(x => x.Type == EmbedType.Rich)?.ToEmbedBuilder().Build() ?? null;
    }
}
