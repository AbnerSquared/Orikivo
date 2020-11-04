using Discord;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Net;

namespace Orikivo
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Determines if this message contains any emotes.
        /// </summary>
        public static bool ContainsEmote(this IMessage message)
            => OriRegex.ContainsEmote(message.Content);

        /// <summary>
        /// Determines if this message contains the specified emote.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <param name="emote">The emote to compare the collection of captured emotes to.</param>
        /// <returns>A value that represents the result of this comparison.</returns>
        public static bool ContainsEmote(this IMessage message, IEmote emote)
            => OriRegex.CaptureEmotes(message.Content).Any(x => x.Equals(emote));

        /// <summary>
        /// Returns a collection of all emotes that this message contains.
        /// </summary>
        public static List<Emote> GetEmotes(this IMessage message)
            => OriRegex.CaptureEmotes(message.Content);

        /// <summary>
        /// Attempts to return an embed of <see cref="EmbedType.Rich"/> from this message.
        /// </summary>
        public static EmbedBuilder GetRichEmbed(this IMessage message)
            => message.Embeds
                .FirstOrDefault(x => x.Type == EmbedType.Rich)?
                .ToEmbedBuilder();

        /// <summary>
        /// Extracts and returns the content of this message.
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
        /// Clones this message to the specified message channel.
        /// </summary>
        public static async Task<IUserMessage> CloneAsync(this IMessage message,
            IMessageChannel channel,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await channel.SendMessageAsync(message.Content, message.IsTTS, message.GetRichEmbed()?.Build(), options, allowedMentions);

        /// <summary>
        /// Modifies this message.
        /// </summary>
        public static async Task ModifyAsync(this IUserMessage message,
            string text = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            await message.ModifyAsync(delegate (MessageProperties x)
            {
                x.Content = !string.IsNullOrWhiteSpace(text) ? text : message.Content;
                x.Embed = embed ?? message.GetRichEmbed()?.Build();
            }, options);
        }

        /// <summary>
        /// Replaces this message with a new copy of itself and modifies it with the specified properties.
        /// </summary>
        public static async Task<IUserMessage> ReplaceAsync(this IUserMessage message,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            string filePath = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false,
            AllowedMentions allowedMentions = null)
        {
            IUserMessage next;

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                next = await message.Channel.SendFileAsync(filePath,
                    !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                    isTTS,
                    embed ?? message.GetRichEmbed()?.Build(),
                    options,
                    isSpoiler,
                    allowedMentions);
            }
            else
            {
                next = await message.Channel.SendMessageAsync(
                    !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                    isTTS,
                    embed ?? message.GetRichEmbed()?.Build(),
                    options,
                    allowedMentions);
            }

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        /// <summary>
        /// Replaces this message with a new copy of itself and modifies it with the specified properties.
        /// </summary>
        public static async Task<IUserMessage> ReplaceAsync(this IUserMessage message,
            System.Drawing.Image image,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false,
            AllowedMentions allowedMentions = null)
        {
            IUserMessage next = await message.Channel.SendImageAsync(image,
                path,
                !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                isTTS,
                embed ?? message.GetRichEmbed()?.Build(),
                format,
                options,
                isSpoiler,
                allowedMentions);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        /// <summary>
        /// Replaces this message with a new copy of itself and modifies it with the specified properties.
        /// </summary>
        public static async Task<IUserMessage> ReplaceAsync(this IUserMessage message,
            MemoryStream gif,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false,
            AllowedMentions allowedMentions = null)
        {
            IUserMessage next = await message.Channel.SendGifAsync(gif,
                path,
                Check.NotNull(text) ? text : message.Content,
                isTTS,
                embed ?? message.GetRichEmbed()?.Build(),
                options,
                isSpoiler,
                allowedMentions);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        /// <summary>
        /// Attempts to delete this message.
        /// </summary>
        public static async Task<bool> TryDeleteAsync(this IMessage message, RequestOptions options = null)
        {
            try
            {
                await message.DeleteAsync(options);
                return true;
            }
            catch (HttpException)
            {
                return false;
            }
        }
    }
}
