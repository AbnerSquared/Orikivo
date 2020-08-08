using Discord;
using Discord.WebSocket;
using Orikivo.Drawing.Encoding;
using Orikivo.Desync;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord.Net;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            MessageContent content,
            RequestOptions options = null)
        {
            return await channel.SendMessageAsync(content.Content, content.IsTTS, content.Embed.Build(), options);
        }

        public static async Task ModifyAsync(this IUserMessage message,
            string text = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            await message.ModifyAsync(delegate (MessageProperties x)
            {
                x.Content = !string.IsNullOrWhiteSpace(text) ? text : x.Content;
                x.Embed = embed ?? x.Embed;
                }, options);
        }

        public static async Task<IUserMessage> ReplaceAsync(this IUserMessage message,
            MemoryStream gif, 
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false)
        {
            IUserMessage next = await message.Channel.SendGifAsync(gif,
                path, 
                Check.NotNull(text) ? text : message.Content,
                isTTS,
                embed ?? message.GetRichEmbed()?.Build(),
                options: options,
                isSpoiler: isSpoiler);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        public static async Task<IUserMessage> ReplaceAsync(this IUserMessage message,
            System.Drawing.Image image,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false)
        {
            IUserMessage next = await message.Channel.SendImageAsync(image,
                path,
                !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                isTTS,
                embed ?? message.GetRichEmbed()?.Build(),
                options: options,
                isSpoiler: isSpoiler);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        public static async Task<IUserMessage> ReplaceAsync(this IUserMessage message,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            string filePath = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false)
        {
            IUserMessage next;

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                next = await message.Channel.SendFileAsync(filePath,
                    !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                    isTTS,
                    embed ?? message.GetRichEmbed()?.Build(),
                    options,
                    isSpoiler);
            }
            else
            {
                next = await message.Channel.SendMessageAsync(
                    !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                    isTTS,
                    embed ?? message.GetRichEmbed()?.Build(),
                    options);
            }

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

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

        // TODO: Use the base Notifier class instead to give it the ability to be generic
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            BaseUser user,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null)
        {
            var content = new StringBuilder();

            if (user.Notifier.CanNotify)
            {
                content.AppendLine(user.Notifier.Notify());
                user.Notifier.LastNotified = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(text))
                content.Append(text);

            return await channel.SendMessageAsync(content.ToString(), isTTS, embed, options);
        }

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            BaseUser user,
            Message message,
            RequestOptions options = null)
        {
            var content = new StringBuilder();

            if (user.Notifier.CanNotify)
            {
                content.AppendLine(user.Notifier.Notify());
                user.Notifier.LastNotified = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(message.Text))
                content.Append(message.Text);

            if (Check.NotNull(message.AttachmentUrl))
                return await channel.SendFileAsync(message.AttachmentUrl, content.ToString(), message.IsTTS, message.Embed, options, message.IsSpoiler);
            else
                return await channel.SendMessageAsync(content.ToString(), message.IsTTS, message.Embed, options);
        }

        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this IMessageChannel channel,
            User user,
            CooldownData cooldown,
            RequestOptions options = null)
            => await MessageUtils.WarnCooldownAsync(channel, user, cooldown, options);

        /// <summary>
        /// Sends an image to the specified channel and disposes of it.
        /// </summary>
        public static async Task<IUserMessage> SendImageAsync(this IMessageChannel channel,
            System.Drawing.Image image,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png,
            RequestOptions options = null,
            bool isSpoiler = false)
            => await MessageUtils.SendImageAsync(channel, image, path, text, isTTS, embed, format, options, isSpoiler);

        /// <summary>
        /// Sends a GIF to the specified channel and disposes of it from a specified <see cref="MemoryStream"/>.
        /// </summary>
        public static async Task<IUserMessage> SendGifAsync(this IMessageChannel channel,
            MemoryStream gif,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            Quality quality = Quality.Bpp8,
            RequestOptions options = null,
            bool isSpoiler = false)
            => await MessageUtils.SendGifAsync(channel, gif, path, text, isTTS, embed, quality, options, isSpoiler);

        /// <summary>
        /// Attempts to warn a user about a global cooldown preventing any command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel,
            User user,
            DateTime globalExpires,
            RequestOptions options = null)
            => await MessageUtils.WarnCooldownAsync(channel, user, globalExpires, options);

        // TODO: Create custom error embed presets and default to this if there isn't one set.
        /// <summary>
        /// Sends a manual error message to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> ThrowAsync(this IMessageChannel channel,
            string error,
            RequestOptions options = null)
            => await MessageUtils.ThrowAsync(channel, error, options);

        public static async Task<IUserMessage> ThrowAsync(this IMessageChannel channel,
            OriError error,
            RequestOptions options = null)
            => await MessageUtils.ThrowAsync(channel, error, options);

        /// <summary>
        /// Catches an <see cref="Exception"/> and sends its information to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> CatchAsync(this IMessageChannel channel,
            Exception ex,
            RequestOptions options = null)
            => await MessageUtils.CatchAsync(channel, ex, options);

        public static async Task<IUserMessage> WarnAsync(this IMessageChannel channel,
            string warning,
            RequestOptions options = null)
            => await channel.SendMessageAsync(Format.Warning(warning), options: options);

        /// <summary>
        /// Sends a custom message object to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            Message message,
            RequestOptions options = null)
            => await MessageUtils.SendMessageAsync(channel, message, options);

        /// <summary>
        /// Gets the URL bound to a voice channel to allow users to participate in screen sharing.
        /// </summary>
        public static string GetUrl(this IVoiceChannel channel)
            => Format.GetVoiceChannelUrl(channel.GuildId, channel.Id);
    }
}
