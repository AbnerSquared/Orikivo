using Discord;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        private static System.Drawing.Imaging.ImageFormat GetImageFormat(Discord.ImageFormat format)
            => format switch
            {
                Discord.ImageFormat.Jpeg => System.Drawing.Imaging.ImageFormat.Jpeg,
                Discord.ImageFormat.Png => System.Drawing.Imaging.ImageFormat.Png,
                Discord.ImageFormat.Gif => System.Drawing.Imaging.ImageFormat.Gif,
                Discord.ImageFormat.Auto => System.Drawing.Imaging.ImageFormat.Png,
                _ => throw new NotSupportedException("The specified ImageFormat is not supported.")
            };

        private static void Save(System.Drawing.Image image, string path, System.Drawing.Imaging.ImageFormat format)
        {
            using (image)
            {
                var encoder = Encoder.Quality;
                EncoderParameter[] args = { new EncoderParameter(encoder, 100) };
                EncoderParameters parameters = new EncoderParameters(args.Length);
                for (int i = 0; i < args.Length; i++)
                    parameters.Param[i] = args[i];
                image.Save(path, GetImageCodec(format), parameters); // bmp can be disposed, as it was already stored
            }
        }

        private static ImageCodecInfo GetImageCodec(System.Drawing.Imaging.ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;

            return null; // No matching codec could be found
        }

        public static async Task<IUserMessage> SendMessageAsync(
            this IMessageChannel channel,
            MessageContent content,
            RequestOptions options = null)
            => await channel.SendMessageAsync(content.Content, content.IsTTS,
                content.Embed.Build(), options);

        public static async Task ModifyAsync(
            this IUserMessage message,
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

        public static async Task<IUserMessage> ReplaceAsync(
            this IUserMessage message,
            Bitmap image,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false)
        {
            IUserMessage next = await message.Channel.SendImageAsync(
                image,
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

        public static async Task<IUserMessage> ReplaceAsync(
            this IUserMessage message,
            string filePath,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            bool deleteLastMessage = false,
            RequestOptions options = null,
            bool isSpoiler = false)
        {
            IUserMessage next = await message.Channel.SendFileAsync(
                filePath,
                !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                isTTS,
                embed ?? message.GetRichEmbed()?.Build(),
                options);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        public static async Task<IUserMessage> ReplaceAsync(
            this IUserMessage message,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            bool deleteLastMessage = false,
            RequestOptions options = null)
        {
            IUserMessage next = await message.Channel.SendMessageAsync(
                !string.IsNullOrWhiteSpace(text) ? text : message.Content,
                isTTS,
                embed ?? message.GetRichEmbed()?.Build(),
                options);

            if (deleteLastMessage)
                await message.DeleteAsync();

            return next;
        }

        /// <summary>
        /// Sends an image to the specified channel and disposes of it.
        /// </summary>
        public static async Task<IUserMessage> SendImageAsync(
            this IMessageChannel channel,
            System.Drawing.Image image,
            string path,
            string text = null,
            bool isTTS = false,
            Embed embed = null,
            Discord.ImageFormat format = Discord.ImageFormat.Png,
            RequestOptions options = null,
            bool isSpoiler = false)
        {
            using (image)
                Save(image, path, GetImageFormat(format));

            if (embed != null)
            {
                if (!embed.Image.HasValue)
                {
                    embed = embed
                        .ToEmbedBuilder()
                        .WithImageUrl($"attachment://{Path.GetFileName(path)}")
                        .Build();
                }
            }

            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler);
        }

        public static async Task<IUserMessage> SendEmbedAsync(
            this IMessageChannel channel,
            Embed embed,
            string message = null,
            bool isTTS = false,
            RequestOptions options = null)
            => await channel.SendMessageAsync(message, isTTS, embed, options);

        /// <summary>
        /// Gets the URL bound to a voice channel to allow users to participate in screen sharing.
        /// </summary>
        public static string GetUrl(this IVoiceChannel channel)
            => string.Format("https://discordapp.com/channels/{0}/{1}", channel.GuildId, channel.Id);
    }
}
