﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class ChannelExtensions
    {
        public static async Task WarnCooldownAsync(this ISocketMessageChannel channel, OriUser user, KeyValuePair<string, CooldownInfo> cooldown)
            => await OriMessageHelper.WarnCooldownAsync(channel, user, cooldown);

        public static async Task ThrowAsync(this ISocketMessageChannel channel, string error, RequestOptions options = null)
            => await OriMessageHelper.ThrowAsync(channel, error, options);

        public static async Task CatchAsync(this ISocketMessageChannel channel, Exception ex, RequestOptions options = null)
            => await OriMessageHelper.CatchAsync(channel, ex, options);

        public static async Task SendMessageAsync(this ISocketMessageChannel channel, OriMessage oriMessage, RequestOptions options = null)
            => await OriMessageHelper.SendMessageAsync(channel, oriMessage, options);
    }
}
