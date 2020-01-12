using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Provides help with extra methods that revolve around sending messages.
    /// </summary>
    public static class MessageUtils
    {
        public static readonly Dictionary<OriError, MessageBuilder> ErrorPresets = new Dictionary<OriError, MessageBuilder>();
        // NotifyAsync() ?? Creates a notification pop-up.

            // TODO: Optimize later on.
        public static async Task NotifyMeritAsync(ISocketMessageChannel channel, User user, IEnumerable<KeyValuePair<string, Merit>> merits)
        {
            if (!user.IsOnCooldown(Cooldown.Notify))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"> You have earned {(merits.Count() > 1 ? "new merits!" : "a new merit!")}");
                sb.AppendJoin("\n", merits.Select(x => "• " + Format.Bold(x.Value.Name)));

                await channel.SendMessageAsync(sb.ToString());
                user.SetCooldown(CooldownType.Global, Cooldown.Notify, TimeSpan.FromSeconds(3.0));
            }
        }

        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(ISocketMessageChannel channel, User user, Unstable.CooldownData cooldown)
            => await WarnCooldownAsync(channel, user, cooldown);

        /// <summary>
        /// Attempts to warn a user about a global cooldown preventing any command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(ISocketMessageChannel channel, User user, DateTime globalExpires)
        {
            if (!user.IsOnCooldown(Cooldown.Notify))
            {
                //RestUserMessage msg = 
                await channel.SendMessageAsync($"You are sending requests too quickly!\nTime left: **{DateTimeUtils.TimeSince(globalExpires).ToString(@"hh\:mm\:ss")}**");
                user.SetCooldown(CooldownType.Global, Cooldown.Notify, TimeSpan.FromSeconds(3.0));
                //await Task.Delay(TimeSpan.FromSeconds(3));
                //await msg.DeleteAsync();
            }
        }

        /// <summary>
        /// Sends a custom message object to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> SendMessageAsync(ISocketMessageChannel channel, Message message, RequestOptions options = null)
        {
            if (Checks.NotNull(message.AttachmentUrl))
                return await channel.SendFileAsync(message.AttachmentUrl, message.Text, message.IsTTS, message.Embed, options);
            else
                return await channel.SendMessageAsync(message.Text, message.IsTTS, message.Embed, options);
        }

        /// <summary>
        /// Sends a manual error message to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> ThrowAsync(ISocketMessageChannel channel, string error, RequestOptions options = null)
            => await channel.SendMessageAsync(OriFormat.Error("Oops!", "An error has occurred.", error), options: options);
        // $"**Oops!**\nAn error has occurred.```{error}```"

        /// <summary>
        /// Sends an error message to the specified channel. If a preset is not defined, it executes the manual variant of ThrowAsync.
        /// </summary>
        public static async Task<RestUserMessage> ThrowAsync(ISocketMessageChannel channel, OriError error, RequestOptions options = null)
        {
            if (!ErrorPresets.ContainsKey(error))
                return await ThrowAsync(channel, error.ToString(), options);

            return await SendMessageAsync(channel, ErrorPresets[error].Build(), options);
        }

        /// <summary>
        /// Catches a possible Exception and sends its information to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> CatchAsync(ISocketMessageChannel channel, Exception ex, RequestOptions options = null)
        {
            string[] errorPaths = ex.StackTrace.Split('\n');
            StringBuilder error = new StringBuilder();

            error.Append("**Yikes!**");
            error.AppendLine();
            error.Append("An exception has been thrown.");

            error.Append("```");
            error.AppendLine();

            error.Append(ex.Message);
            error.AppendLine();
            error.Append("```");
            error.AppendLine();

            error.Append("```bf");
            error.AppendLine();

            for (int i = 0; i < errorPaths.Length; i++)
            {
                if ((error.Length + errorPaths[i].Length) < 1997)
                {
                    error.AppendLine(errorPaths[i]);
                }
                else
                {
                    break;
                }
            }

            error.Append("```");

            // OriFormat.Error("Yikes!", "An exception has been thrown.", ex.Message, ex.StackTrace/*?.Split('\n')[0]*/)
            return await channel.SendMessageAsync(error.ToString(), options: options);
        }
        // $"**Yikes!**\nAn exception has been thrown.```{ex.Message}```\n```bf\n{ex.StackTrace}```"

        /// <summary>
        /// Sends an Embed to the specified channel.
        /// </summary>
        public static async Task<RestUserMessage> SendEmbedAsync(ISocketMessageChannel channel, Embed embed, string message = null, bool isTTS = false, RequestOptions options = null)
            => await channel.SendMessageAsync(message, isTTS, embed, options);
    }
}
