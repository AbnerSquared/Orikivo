using Discord;
using Discord.WebSocket;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Orikivo;

namespace Arcadia
{
    public static class CoreService
    {
        private static readonly string OnBefore = "🌌 **Pinging...**";
        private static readonly string OnAfter = "🏓 **Pong!**";

        public static async Task<IUserMessage> PingAsync(IMessageChannel channel, BaseSocketClient client)
        {
            var stopwatch = Stopwatch.StartNew();
            IUserMessage message = await channel.SendMessageAsync(OnBefore);
            stopwatch.Stop();

            var result = new StringBuilder();
            result.AppendLine(OnAfter);
            result.AppendLine($"> **Internal** {GetCounter(stopwatch.ElapsedMilliseconds)}");
            result.AppendLine($"> **Gateway** {GetCounter(client.Latency)}");

            await message.ModifyAsync(result.ToString());
            return message;
        }

        private static string GetCounter(double ms)
            => $"(**{ms}**ms)";
    }
}
