using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orikivo.Drawing;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        /// <summary>
        /// Sends a message to this message channel.
        /// </summary>
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            Embed embed,
            string text = null,
            bool isTTS = false,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            return await channel.SendMessageAsync(text, isTTS, embed, options, allowedMentions);
        }

        /// <summary>
        /// Sends a message to this message channel.
        /// </summary>
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            MessageContent content,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            return await channel.SendMessageAsync(content.Content, content.IsTTS, content.Embed.Build(), options, allowedMentions);
        }

        /// <summary>
        /// Sends a message object to this message channel.
        /// </summary>
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            Message message,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            if (Check.NotNull(message.AttachmentUrl))
                return await channel.SendFileAsync(message.AttachmentUrl, message.Text, message.IsTTS, message.Embed, options, message.IsSpoiler, allowedMentions);

            return await channel.SendMessageAsync(message.Text, message.IsTTS, message.Embed, options, allowedMentions);
        }

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            BaseUser user,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            var content = new StringBuilder();

            if (user.Notifier.CanNotify)
            {
                text ??= string.Empty;
                content.AppendLine(user.Notifier.Notify(2000 - text.Length));
                user.Notifier.LastNotified = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(text))
                content.Append(text);

            return await channel.SendMessageAsync(content.ToString(), isTTS, embed, options, allowedMentions);
        }

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            BaseUser user,
            Message message,
            RequestOptions options = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var content = new StringBuilder();

            if (user.Notifier.CanNotify)
            {
                content.AppendLine(user.Notifier.Notify(2000 - (message.Text?.Length ?? 0)));
                user.Notifier.LastNotified = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(message.Text))
                content.Append(message.Text);

            if (!string.IsNullOrWhiteSpace(message.AttachmentUrl))
                return await channel.SendFileAsync(message.AttachmentUrl, content.ToString(), message.IsTTS, message.Embed, options, message.IsSpoiler);

            return await channel.SendMessageAsync(content.ToString(), message.IsTTS, message.Embed, options);
        }

        /// <summary>
        /// Sends an attachment to this message channel.
        /// </summary>
        public static async Task<IUserMessage> SendAttachmentAsync(this IMessageChannel channel,
            IAttachment attachment,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            bool isSpoiler = false,
            AllowedMentions allowedMentions = null)
        {
            return await channel.SendFileAsync(attachment.Url, text, isTTS, embed, options, isSpoiler, allowedMentions);
        }

        /// <summary>
        /// Sends an image to this message channel and disposes of it.
        /// </summary>
        public static async Task<IUserMessage> SendImageAsync(this IMessageChannel channel,
            Image image,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png,
            RequestOptions options = null,
            bool isSpoiler = false,
            AllowedMentions allowedMentions = null)
        {
            using (image)
                ImageHelper.Save(image, path, GetImageFormat(format));

            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler, allowedMentions);
        }

        /// <summary>
        /// Sends a GIF to this message channel and disposes of it.
        /// </summary>
        public static async Task<IUserMessage> SendGifAsync(this IMessageChannel channel,
            MemoryStream gif,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null,
            bool isSpoiler = false,
            AllowedMentions allowedMentions = null)
        {
            using (Image img = Image.FromStream(gif))
                img.Save(path, GetImageFormat(GraphicsFormat.Gif));

            await gif.DisposeAsync();
            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler, allowedMentions);
        }

        /// <summary>
        /// Sends an error message to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> ThrowAsync(this IMessageChannel channel,
            string error,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            return await channel.SendMessageAsync(Format.Error("Oops!", "An error has occurred.", error), options: options, allowedMentions: allowedMentions);
        }

        /// <summary>
        /// Catches an <see cref="Exception"/> and sends its information to this message channel.
        /// </summary>
        public static async Task<IUserMessage> CatchAsync(this IMessageChannel channel,
            Exception ex,
            StackTraceMode stackTraceMode = StackTraceMode.Simple,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            var error = new StringBuilder();

            error.AppendLine("> **Yikes!**");
            error.AppendLine("> An exception has been thrown.");

            if (ex == null)
                return await channel.SendMessageAsync(error.ToString(), options: options, allowedMentions: allowedMentions);

            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                error.AppendLine("```");
                error.AppendLine(ex.Message);
                error.Append("```");
            }

            if (string.IsNullOrWhiteSpace(ex.StackTrace) || stackTraceMode == StackTraceMode.None)
                return await channel.SendMessageAsync(error.ToString(), options: options, allowedMentions: allowedMentions);

            error.AppendLine();
            error.Append(GetStackTrace(ex.StackTrace, stackTraceMode, error.Length));

            return await channel.SendMessageAsync(error.ToString(), options: options, allowedMentions: allowedMentions);
        }

        /// <summary>
        /// Gets the URL bound to this voice channel to allow users to participate in screen sharing.
        /// </summary>
        public static string GetUrl(this IVoiceChannel channel)
        {
            return Format.GetVoiceChannelUrl(channel.GuildId, channel.Id);
        }

        private static string GetStackTrace(string stackTrace, StackTraceMode traceMode, int messageLength)
        {
            if (traceMode == StackTraceMode.None || string.IsNullOrWhiteSpace(stackTrace))
                return null;

            var result = new StringBuilder();
            result.AppendLine($"> **Stack Trace**");

            switch (traceMode)
            {
                case StackTraceMode.Simple:
                    if (!StackTracePath.TryParse(stackTrace, out List<StackTracePath> paths))
                        return null;

                    StackTracePath path = paths.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Path)) ?? paths.First();

                    if (path.Method.Length + messageLength + 7 >= 2000)
                        break;

                    result.AppendLine($"• at `{path.Method}`");

                    if (string.IsNullOrWhiteSpace(path.Path))
                        break;

                    string marker = path.LineIndex.HasValue ? $" (Line **{path.LineIndex.Value:##,0}**)" : "";

                    if (marker.Length + path.Path.Length + messageLength + 7 >= 2000)
                        break;

                    result.AppendLine($"• in `{path.Path}`{marker}");

                    break;

                default:
                    string[] errorPaths = stackTrace.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    if (errorPaths.Length == 0 || messageLength + result.Length + errorPaths[0].Length + 8 >= 2000)
                        break;

                    result.AppendLine("```bf");

                    foreach (string errorPath in errorPaths)
                    {
                        if (messageLength + result.Length + errorPath.Length + 3 >= 2000)
                            break;

                        result.AppendLine(errorPath);
                    }

                    result.Append("```");

                    break;
            }

            return result.ToString();
        }

        private static ImageFormat GetImageFormat(GraphicsFormat format)
        {
            return format switch
            {
                GraphicsFormat.Gif => ImageFormat.Gif,
                _ => ImageFormat.Png
            };
        }
    }
}
