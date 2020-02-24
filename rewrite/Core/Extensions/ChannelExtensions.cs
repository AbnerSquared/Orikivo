using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Orikivo.Drawing.Encoding;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        public static async Task NotifyMeritAsync(this ISocketMessageChannel channel, User user, IEnumerable<Merit> merits)
            => await MessageUtils.NotifyMeritAsync(channel, user, merits);

        // i don't know if this actually goes through
        public static async Task<Embed> GetEmbedAsync(this RestUserMessage message)
        {
            Embed embed = null;
            await message.ModifyAsync(x => embed = x.Embed.GetValueOrDefault(null));

            return embed;
        }

        public static async Task ModifyAsync(this RestUserMessage message, string text = null, Embed embed = null,
            RequestOptions options = null)
        {
            await message.ModifyAsync(delegate (MessageProperties x)
            {
                x.Content = Check.NotNull(text) ? text : x.Content;
                x.Embed = Check.NotNull(embed) ? embed : x.Embed;
                }, options);
        }

        public static async Task<RestUserMessage> ReplaceAsync(this RestUserMessage message, MemoryStream gif, 
            string path, string text = null, bool isTTS = false, Embed embed = null, bool deleteLastMessage = false,
            RequestOptions options = null, bool isSpoiler = false)
        {
            RestUserMessage next = await ((SocketTextChannel)message.Channel).SendGifAsync(gif, path, Check.NotNull(text) ? text : message.Content, isTTS, Check.NotNull(embed) ? embed : message.Embeds.FirstOrDefault(x => x.Type == EmbedType.Rich), options: options, isSpoiler: isSpoiler);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        public static async Task<RestUserMessage> ReplaceAsync(this RestUserMessage message, Bitmap image,
            string path, string text = null, bool isTTS = false, Embed embed = null, bool deleteLastMessage = false,
            RequestOptions options = null, bool isSpoiler = false)
        {
            RestUserMessage next = await ((SocketTextChannel)message.Channel).SendImageAsync(image, path, Check.NotNull(text) ? text : message.Content, isTTS, Check.NotNull(embed) ? embed : message.Embeds.FirstOrDefault(x => x.Type == EmbedType.Rich), options: options, isSpoiler: isSpoiler);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        public static async Task<RestUserMessage> ReplaceAsync(this RestUserMessage message, string filePath,
            string text = null, bool isTTS = false, Embed embed = null, bool deleteLastMessage = false,
            RequestOptions options = null, bool isSpoiler = false)
        {
            RestUserMessage next = await ((SocketTextChannel)message.Channel).SendFileAsync(filePath, Check.NotNull(text) ? text : message.Content, isTTS, Check.NotNull(embed) ? embed : message.Embeds.FirstOrDefault(x => x.Type == EmbedType.Rich), options);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        public static async Task<RestUserMessage> ReplaceAsync(this RestUserMessage message, string text = null,
            bool isTTS = false, Embed embed = null, bool deleteLastMessage = false, RequestOptions options = null)
        {
            RestUserMessage next = await ((SocketTextChannel)message.Channel).SendMessageAsync(Check.NotNull(text) ? text : message.Content, isTTS, Check.NotNull(embed) ? embed : message.Embeds.FirstOrDefault(x => x.Type == EmbedType.Rich), options);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel, User user, CooldownData cooldown, RequestOptions options = null)
            => await MessageUtils.WarnCooldownAsync(channel, user, cooldown, options);

        /// <summary>
        /// Sends an image to the specified channel and disposes of it.
        /// </summary>
        public static async Task<RestUserMessage> SendImageAsync(this ISocketMessageChannel channel,
            Bitmap bmp, string path, string text = null, bool isTTS = false, Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png, RequestOptions options = null, bool isSpoiler = false)
            => await MessageUtils.SendImageAsync(channel, bmp, path, text, isTTS, embed, format, options, isSpoiler);

        /// <summary>
        /// Sends a GIF to the specified channel and disposes of it from a specified <see cref="MemoryStream"/>.
        /// </summary>
        public static async Task<RestUserMessage> SendGifAsync(this ISocketMessageChannel channel,
            MemoryStream gif, string path, string text = null, bool isTTS = false, Embed embed = null,
            Quality quality = Quality.Bpp8, RequestOptions options = null, bool isSpoiler = false)
            => await MessageUtils.SendGifAsync(channel, gif, path, text, isTTS, embed, quality, options, isSpoiler);

        /// <summary>
        /// Attempts to warn a user about a global cooldown preventing any command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel, User user, DateTime globalExpires, RequestOptions options = null)
            => await MessageUtils.WarnCooldownAsync(channel, user, globalExpires, options);

        // TODO: Create custom error embed presets and default to this if there isn't one set.
        /// <summary>
        /// Sends a manual error message to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> ThrowAsync(this ISocketMessageChannel channel, string error, RequestOptions options = null)
            => await MessageUtils.ThrowAsync(channel, error, options);

        public static async Task<RestUserMessage> ThrowAsync(this ISocketMessageChannel channel, OriError error, RequestOptions options = null)
            => await MessageUtils.ThrowAsync(channel, error, options);

        /// <summary>
        /// Catches a possible Exception and sends its information to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> CatchAsync(this ISocketMessageChannel channel, Exception ex, RequestOptions options = null)
            => await MessageUtils.CatchAsync(channel, ex, options);

        /// <summary>
        /// Sends a custom message object to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> SendMessageAsync(this ISocketMessageChannel channel, Message message, RequestOptions options = null)
            => await MessageUtils.SendMessageAsync(channel, message, options);

        /// <summary>
        /// Gets the URL bound to a voice channel to allow users to participate in screen sharing.
        /// </summary>
        public static string GetUrl(this IVoiceChannel channel)
            => OriFormat.GetVoiceChannelUrl(channel.GuildId, channel.Id);
    }
}
