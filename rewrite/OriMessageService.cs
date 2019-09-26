using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // dynamic commands
    // dynamic requires: a timeout, inner command responses, ability to listen to default command handle,
    // edit message or send new message based on messages sent during the timeout
    // used to make messaging simpler for orikivo
    public class OriMessageService
    {
        // make sure to check if the bot can delete messages in the specified channel
        public static async Task WarnCooldownAsync(ISocketMessageChannel channel, OriUser user, KeyValuePair<string, CooldownInfo> cooldown)
        {
            if (user.IsOnCooldown(Cooldown.Notify))
                return;
            else
            {
                
                //RestUserMessage msg = 
                await channel.SendMessageAsync($"You are on cooldown.\nTime left: **{cooldown.Value.TimeLeft.ToString(@"hh\:mm\:ss")}**");
                user.SetCooldown(Cooldown.Notify, 3.0);
                //await Task.Delay(300);
                //await msg.DeleteAsync();
            }
        }

        // static properties
        public static async Task SendMessageAsync(ISocketMessageChannel channel, OriMessage oriMessage, RequestOptions options = null)
            => await channel.SendMessageAsync(oriMessage.Text, oriMessage.IsTTS, oriMessage.Embed?.Build(), options);
        
        // throws an error with the specified reason and summary
        public static async Task ThrowAsync(ISocketMessageChannel channel, string error, RequestOptions options = null)
            => await channel.SendMessageAsync($"**Oops!**\nAn error has occured.```{error}```", options: options);

        public static async Task CatchAsync(ISocketMessageChannel channel, Exception ex, RequestOptions options = null)
            => await channel.SendMessageAsync($"**Yikes!**\nAn exception has been thrown.```{ex.Message}```\n```bf\n{ex.StackTrace}```", options: options);

        public static async Task SendEmbedAsync(ISocketMessageChannel channel, Embed embed, string message = null, bool isTTS = false, RequestOptions options = null)
            => await channel.SendMessageAsync(message, isTTS, embed, options);
    }
}
