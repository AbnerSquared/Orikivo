using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel, OriUser user, KeyValuePair<string, CooldownInfo> cooldown)
            => await OriMessageHelper.WarnCooldownAsync(channel, user, cooldown);

        // TODO: Create custom error embed presets and default to this if there isn't one set.
        /// <summary>
        /// Sends a custom error message to the specified channel.
        /// </summary>
        public static async Task ThrowAsync(this ISocketMessageChannel channel, string error, RequestOptions options = null)
            => await OriMessageHelper.ThrowAsync(channel, error, options);

        /// <summary>
        /// Catches a possible Exception and sends its information to the specified channel.
        /// </summary>
        public static async Task CatchAsync(this ISocketMessageChannel channel, Exception ex, RequestOptions options = null)
            => await OriMessageHelper.CatchAsync(channel, ex, options);

        /// <summary>
        /// Sends a custom message object to the specified channel.
        /// </summary>
        public static async Task SendMessageAsync(this ISocketMessageChannel channel, OriMessage oriMessage, RequestOptions options = null)
            => await OriMessageHelper.SendMessageAsync(channel, oriMessage, options);
    }
}
