using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Orikivo.Drawing.Encoding;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        public static async Task NotifyMeritAsync(this ISocketMessageChannel channel, User user, IEnumerable<Merit> merits)
            => await MessageUtils.NotifyMeritAsync(channel, user, merits);

        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel, User user, CooldownData cooldown)
            => await MessageUtils.WarnCooldownAsync(channel, user, cooldown);

        /// <summary>
        /// Sends an image to the specified channel and disposes of it.
        /// </summary>
        public static async Task<RestUserMessage> SendImageAsync(this ISocketMessageChannel channel, Bitmap bmp, string path,
            GraphicsFormat format = GraphicsFormat.Png, RequestOptions options = null)
            => await MessageUtils.SendImageAsync(channel, bmp, path, format, options);

        /// <summary>
        /// Sends a GIF to the specified channel and disposes of it from a specified <see cref="MemoryStream"/>.
        /// </summary>
        public static async Task<RestUserMessage> SendGifAsync(this ISocketMessageChannel channel, MemoryStream gif, string path,
            Quality quality = Quality.Bpp8, RequestOptions options = null)
            => await MessageUtils.SendGifAsync(channel, gif, path, quality, options);

        /// <summary>
        /// Attempts to warn a user about a global cooldown preventing any command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel, User user, DateTime globalExpires)
            => await MessageUtils.WarnCooldownAsync(channel, user, globalExpires);

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
