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
using Path = System.IO.Path;
using MongoDB.Driver;

namespace Orikivo
{
    public static class ChannelExtensions
    {
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

        public static async Task RespondAsync(this IDiscordInteraction interaction,
            Message message,
            bool ephemeral = false,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            if (Check.NotNull(message.AttachmentUrl))
                await interaction.RespondWithFileAsync(message.AttachmentUrl, message.Text, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
            else
                await interaction.RespondAsync(message.Text, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<IUserMessage> FollowupAsync(this IDiscordInteraction interaction,
            Message message,
            bool ephemeral = false,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            if (Check.NotNull(message.AttachmentUrl))
                return await interaction.FollowupWithFileAsync(message.AttachmentUrl, message.Text, isTTS: message.IsTTS, embed: message.Embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);

            return await interaction.FollowupAsync(message.Text, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
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

        public static async Task RespondAsync(this IDiscordInteraction interaction,
        BaseUser user,
        string text = null,
        bool isTTS = false,
        bool ephemeral = false,
        Embed embed = null,
        RequestOptions options = null,
        AllowedMentions allowedMentions = null)
        => await interaction.RespondAsync(BuildUserMessage(user, text), isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
       
        public static async Task<IUserMessage> FollowupAsync(this IDiscordInteraction interaction,
        BaseUser user,
        string text = null,
        bool isTTS = false,
        bool ephemeral = false,
        Embed embed = null,
        RequestOptions options = null,
        AllowedMentions allowedMentions = null)
        {
            return await interaction.FollowupAsync(BuildUserMessage(user, text), isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
        BaseUser user,
        string text = null,
        bool isTTS = false,
        Embed embed = null,
        RequestOptions options = null,
        AllowedMentions allowedMentions = null)
        {
            return await channel.SendMessageAsync(BuildUserMessage(user, text), isTTS, embed, options, allowedMentions);
        }

        public static async Task RespondAsync(this IDiscordInteraction interaction,
        BaseUser user,
        Message message,
        bool ephemeral = false,
        RequestOptions options = null)
        {
            string content = BuildUserMessage(user, message);

            if (!string.IsNullOrWhiteSpace(message.AttachmentUrl))
                await interaction.RespondWithFileAsync(message.AttachmentUrl, content, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options).ConfigureAwait(continueOnCapturedContext: false);
            else
                await interaction.RespondAsync(content, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<IUserMessage> FollowupAsync(this IDiscordInteraction interaction,
        BaseUser user,
        Message message,
        bool ephemeral = false,
        RequestOptions options = null)
        {
            string content = BuildUserMessage(user, message);

            if (!string.IsNullOrWhiteSpace(message.AttachmentUrl))
                return await interaction.FollowupWithFileAsync(message.AttachmentUrl, content, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options).ConfigureAwait(continueOnCapturedContext: false);

            return await interaction.FollowupAsync(content, isTTS: message.IsTTS, ephemeral: ephemeral, embed: message.Embed, options: options).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel,
            BaseUser user,
            Message message,
            RequestOptions options = null)
        {
            string content = BuildUserMessage(user, message);

            if (!string.IsNullOrWhiteSpace(message.AttachmentUrl))
                return await channel.SendFileAsync(message.AttachmentUrl, content, message.IsTTS, message.Embed, options, message.IsSpoiler);

            return await channel.SendMessageAsync(content, message.IsTTS, message.Embed, options);
        }

        

        public static async Task RespondWithAttachmentAsync(this IDiscordInteraction interaction,
            IAttachment attachment,
            string text = null,
            bool isTTS = false,
            bool ephemeral = false,
            Embed embed = null,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await interaction.RespondWithFileAsync(attachment.Url, text, isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions);

        public static async Task<IUserMessage> FollowupWithAttachmentAsync(this IDiscordInteraction interaction,
            IAttachment attachment,
            string text = null,
            bool isTTS = false,
            bool ephemeral = false,
            Embed embed = null,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await interaction.FollowupWithFileAsync(attachment.Url, text, isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions);


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
            => await channel.SendFileAsync(attachment.Url, text, isTTS, embed, options, isSpoiler, allowedMentions);

        public static async Task RespondWithImageAsync(this IDiscordInteraction interaction,
            Image image,
            string path,
            string text = null,
            bool isTTS = false,
            bool ephemeral = false,
            Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            SaveImage(image, path, format);
            await interaction.RespondWithFileAsync(path, text, isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<IUserMessage> FollowupWithImageAsync(this IDiscordInteraction interaction,
            Image image,
            string path,
            string text = null,
            bool isTTS = false,
            bool ephemeral = false,
            Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {

            SaveImage(image, path, format);
            return await interaction.FollowupWithFileAsync(path, text, isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
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
            SaveImage(image, path, format);
            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler, allowedMentions);
        }

        public static async Task RespondWithGifAsync(this IDiscordInteraction interaction,
            MemoryStream gif,
            string path,
            string text = null,
            bool isTTS = false,
            bool ephemeral = false,
            Embed embed = null,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            await SaveGifAsync(gif, path);
            await interaction.RespondWithFileAsync(path, text, isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<IUserMessage> FollowupWithGifAsync(this IDiscordInteraction interaction,
            MemoryStream gif,
            string path,
            string text = null,
            bool isTTS = false,
            bool ephemeral = false,
            Embed embed = null,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
        {
            await SaveGifAsync(gif, path);
            return await interaction.FollowupWithFileAsync(path, text, isTTS: isTTS, ephemeral: ephemeral, embed: embed, options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);
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
            await SaveGifAsync(gif, path);
            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler, allowedMentions);
        }

        public static async Task RespondWithErrorAsync(this IDiscordInteraction interaction,
            string error,
            bool ephemeral = false,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await interaction.RespondAsync(Format.Error("Oops!", "An error has occurred.", error), ephemeral: ephemeral, options: options, allowedMentions: allowedMentions);

        public static async Task<IUserMessage> FollowupWithErrorAsync(this IDiscordInteraction interaction,
            string error,
            bool ephemeral = false,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await interaction.FollowupAsync(Format.Error("Oops!", "An error has occurred.", error), ephemeral: ephemeral, options: options, allowedMentions: allowedMentions);

        /// <summary>
        /// Sends an error message to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> ThrowAsync(this IMessageChannel channel,
            string error,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await channel.SendMessageAsync(Format.Error("Oops!", "An error has occurred.", error), options: options, allowedMentions: allowedMentions);

        public static async Task RespondWithErrorAsync(this IDiscordInteraction interaction,
            Exception ex,
            StackTraceMode stackTraceMode = StackTraceMode.Simple,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await interaction.RespondAsync(CreateStackTrace(ex, stackTraceMode), options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);

        public static async Task<IUserMessage> FollowupWithErrorAsync(this IDiscordInteraction interaction,
            Exception ex,
            StackTraceMode stackTraceMode = StackTraceMode.Simple,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await interaction.FollowupAsync(CreateStackTrace(ex, stackTraceMode), options: options, allowedMentions: allowedMentions).ConfigureAwait(continueOnCapturedContext: false);

        public static async Task<IUserMessage> CatchAsync(this IMessageChannel channel,
            Exception ex,
            StackTraceMode stackTraceMode = StackTraceMode.Simple,
            RequestOptions options = null,
            AllowedMentions allowedMentions = null)
            => await channel.SendMessageAsync(CreateStackTrace(ex, stackTraceMode), options: options, allowedMentions: allowedMentions);

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

        private static string CreateStackTrace(Exception ex, StackTraceMode stackTraceMode)
        {
            var error = new StringBuilder();

            error.AppendLine("> **Yikes!**");
            error.AppendLine("> An exception has been thrown.");

            if (ex == null)
                return error.ToString();

            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                error.AppendLine("```");
                error.AppendLine(ex.Message);
                error.Append("```");
            }

            if (string.IsNullOrWhiteSpace(ex.StackTrace) || stackTraceMode == StackTraceMode.None)
                return error.ToString();

            error.AppendLine();
            error.Append(GetStackTrace(ex.StackTrace, stackTraceMode, error.Length));

            return error.ToString();
        }

        private static async Task SaveGifAsync(MemoryStream gif, string path)
        {
            using (Image img = Image.FromStream(gif))
                img.Save(path, GetImageFormat(GraphicsFormat.Gif));

            await gif.DisposeAsync();
        }

        private static void SaveImage(Image image, string path, GraphicsFormat format)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (image)
                ImageHelper.Save(image, path, GetImageFormat(format));
        }

        private static string BuildUserMessage(BaseUser user, Message message)
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

            return content.ToString();
        }

        private static string BuildUserMessage(BaseUser user, string text)
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

            return content.ToString();
        }
    }
}
