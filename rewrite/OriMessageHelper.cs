using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Provides help with extra methods that revolve around sending messages.
    /// </summary>
    public static class OriMessageHelper
    {
        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(ISocketMessageChannel channel, OriUser user, KeyValuePair<string, CooldownInfo> cooldown)
        {
            if (!user.IsOnCooldown(Cooldown.Notify))
            {
                //RestUserMessage msg = 
                await channel.SendMessageAsync($"You are on cooldown.\nTime left: **{cooldown.Value.TimeLeft.ToString(@"hh\:mm\:ss")}**");
                user.SetCooldown(Cooldown.Notify, 3.0);
                //await Task.Delay(TimeSpan.FromSeconds(3));
                //await msg.DeleteAsync();
            }
        }

        /// <summary>
        /// Sends a custom message object to the specified channel.
        /// </summary>
        public static async Task SendMessageAsync(ISocketMessageChannel channel, Message message, RequestOptions options = null)
        {
            if (Checks.NotNull(message.AttachmentUrl))
                await channel.SendFileAsync(message.AttachmentUrl, message.Text, message.IsTTS, message.Embed, options);
            else
                await channel.SendMessageAsync(message.Text, message.IsTTS, message.Embed, options);
        }

        /// <summary>
        /// Sends a custom error message to the specified channel.
        /// </summary>
        public static async Task ThrowAsync(ISocketMessageChannel channel, string error, RequestOptions options = null)
            => await channel.SendMessageAsync($"**Oops!**\nAn error has occured.```{error}```", options: options);

        /// <summary>
        /// Catches a possible Exception and sends its information to the specified channel.
        /// </summary>
        public static async Task CatchAsync(ISocketMessageChannel channel, Exception ex, RequestOptions options = null)
            => await channel.SendMessageAsync($"**Yikes!**\nAn exception has been thrown.```{ex.Message}```\n```bf\n{ex.StackTrace}```", options: options);

        /// <summary>
        /// Sends an Embed to the specified channel.
        /// </summary>
        public static async Task SendEmbedAsync(ISocketMessageChannel channel, Embed embed, string message = null, bool isTTS = false, RequestOptions options = null)
            => await channel.SendMessageAsync(message, isTTS, embed, options);
    }
}
