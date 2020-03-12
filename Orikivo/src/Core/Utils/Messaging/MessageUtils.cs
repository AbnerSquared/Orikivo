﻿using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Orikivo.Drawing;
using Orikivo.Drawing.Encoding;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Image = System.Drawing.Image;

namespace Orikivo
{
    // TODO: Handle notifiers outside of MessageUtils
    /// <summary>
    /// Provides help with extra methods that revolve around sending messages.
    /// </summary>
    public static class MessageUtils
    {
        private static System.Drawing.Imaging.ImageFormat GetImageFormat(GraphicsFormat format)
            => format switch
            {
                GraphicsFormat.Png => System.Drawing.Imaging.ImageFormat.Png,
                GraphicsFormat.Gif => System.Drawing.Imaging.ImageFormat.Gif,
                _ => System.Drawing.Imaging.ImageFormat.Png
            };

        public static readonly Dictionary<OriError, MessageBuilder> ErrorPresets = new Dictionary<OriError, MessageBuilder>();
        // NotifyAsync() ?? Creates a notification pop-up.



        // TODO: Rather than notify instantly, append the notifier for the next message the user calls.
        public static async Task NotifyMeritAsync(IMessageChannel channel, User user, IEnumerable<Merit> merits)
        {
            if (!user.IsOnCooldown(Cooldowns.Notify))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"> You have earned {(merits.Count() > 1 ? "new merits!" : "a new merit!")}");
                sb.AppendJoin("\n", merits.Select(x => "• " + Format.Bold(x.Name)));

                await channel.SendMessageAsync(sb.ToString());
                user.SetCooldown(CooldownType.Global, Cooldowns.Notify, TimeSpan.FromSeconds(3.0));
            }
        }

        /// <summary>
        /// Attempts to warn a user about a cooldown that is currently preventing command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(IMessageChannel channel, User user, CooldownData cooldown, RequestOptions options = null)
            => await WarnCooldownAsync(channel, user, cooldown.ExpiresOn, options);

        /// <summary>
        /// Attempts to warn a user about a global cooldown preventing any command execution.
        /// </summary>
        public static async Task WarnCooldownAsync(IMessageChannel channel, User user, DateTime globalExpires, RequestOptions options = null)
        {
            if (!user.IsOnCooldown(Cooldowns.Notify))
            {
                //RestUserMessage msg = 
                await channel.SendMessageAsync($"You are sending requests too quickly!\nTime left: **{DateTimeUtils.TimeSince(globalExpires).ToString(@"hh\:mm\:ss")}**", options: options);
                user.SetCooldown(CooldownType.Global, Cooldowns.Notify, TimeSpan.FromSeconds(3.0));
                //await Task.Delay(TimeSpan.FromSeconds(3));
                //await msg.DeleteAsync();
            }
        }

        /// <summary>
        /// Sends a custom message object to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> SendMessageAsync(IMessageChannel channel, Message message, RequestOptions options = null)
        {
            if (Check.NotNull(message.AttachmentUrl))
                return await channel.SendFileAsync(message.AttachmentUrl, message.Text, message.IsTTS, message.Embed, options, message.IsSpoiler);
            else
                return await channel.SendMessageAsync(message.Text, message.IsTTS, message.Embed, options);
        }


        
        /// <summary>
        /// Sends an image to the specified channel and disposes of it.
        /// </summary>
        public static async Task<IUserMessage> SendImageAsync(IMessageChannel channel,
            Bitmap bmp, string path, string text = null, bool isTTS = false, Embed embed = null,
            GraphicsFormat format = GraphicsFormat.Png, RequestOptions options = null, bool isSpoiler = false)
        {
            using (bmp)
                BitmapHandler.Save(bmp, path, GetImageFormat(format));

            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler);
        }

        /// <summary>
        /// Sends a GIF to the specified channel and disposes of it from a specified <see cref="MemoryStream"/>.
        /// </summary>
        public static async Task<IUserMessage> SendGifAsync(IMessageChannel channel, MemoryStream gif, string path,
            string text = null, bool isTTS = false, Embed embed = null,
            Quality quality = Quality.Bpp8, RequestOptions options = null, bool isSpoiler = false)
        {
            using (Image img = Image.FromStream(gif))
                img.Save(path, GetImageFormat(GraphicsFormat.Gif));
                //using (Image quantized = EncodeUtils.Quantize(img, quality))
                //    BitmapHandler.Save(quantized, path, GetImageFormat(GraphicsFormat.Gif));
            gif.Dispose();


            return await channel.SendFileAsync(path, text, isTTS, embed, options, isSpoiler);
        }

        /// <summary>
        /// Sends a manual error message to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> ThrowAsync(IMessageChannel channel, string error, RequestOptions options = null)
            => await channel.SendMessageAsync(OriFormat.Error("Oops!", "An error has occurred.", error), options: options);
        // $"**Oops!**\nAn error has occurred.```{error}```"

        /// <summary>
        /// Sends an error message to the specified channel. If a preset is not defined, it executes the manual variant of ThrowAsync.
        /// </summary>
        public static async Task<IUserMessage> ThrowAsync(IMessageChannel channel, OriError error, RequestOptions options = null)
        {
            if (!ErrorPresets.ContainsKey(error))
                return await ThrowAsync(channel, error.ToString(), options);

            return await SendMessageAsync(channel, ErrorPresets[error].Build(), options);
        }


        // TODO: If the stack trace is too large, send the exception to a .txt file.
        /// <summary>
        /// Catches a possible Exception and sends its information to the specified channel.
        /// </summary>
        public static async Task<IUserMessage> CatchAsync(IMessageChannel channel, Exception ex, RequestOptions options = null)
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