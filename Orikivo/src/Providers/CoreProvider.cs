﻿using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class CoreProvider
    {
        private static readonly string _before = "🌌 **Pinging...**";
        private static readonly string _internal = "**Internal**";
        private static readonly string _gateway = "**Gateway**";
        private static readonly string _after = "🏓 **Pong!**";
        // pings to the specified channel.
        public static async Task<IUserMessage> PingAsync(IMessageChannel channel, BaseSocketClient client)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IUserMessage message = await channel.SendMessageAsync(_before);
            stopwatch.Stop();

            StringBuilder result = new StringBuilder();
            result.AppendLine(_after);
            result.AppendLine($"> **Internal** {GetCounter(stopwatch.ElapsedMilliseconds)}");
            result.AppendLine($"> **Gateway** {GetCounter(client.Latency)}");

            await message.ModifyAsync(result.ToString());
            return message;
        }

        private static string GetCounter(double ms)
            => $"(**{ms}**ms)";
    }
}
