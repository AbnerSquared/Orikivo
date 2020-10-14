using System;
using System.Threading.Tasks;
using Arcadia.Casino;
using Discord.Addons.Collectors;
using Discord.WebSocket;
using Orikivo;

namespace Arcadia.Modules
{
    public class CasinoService
    {
        private readonly MessageCollector _collector;
        public CasinoService(DiscordSocketClient client)
        {
            _collector = new MessageCollector(client);
        }

        public async Task RunBlackJackAsync(ArcadeUser invoker, ISocketMessageChannel channel, Wager wager)
        {
            if (!invoker.CanGamble)
                return;

            if (wager.Value < 0)
            {
                await channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (wager.Value == 0)
            {
                await channel.SendMessageAsync($"> ⚠️ You need to specify a positive amount of **Chips** to bet.");
                return;
            }

            if (wager.Value > invoker.ChipBalance)
            {
                await channel.SendMessageAsync($"> ⚠️ You don't have enough **Chips** to bet with.");
                return;
            }

            invoker.CanGamble = false;
            var session = new BlackJackSession(invoker, channel, wager.Value);

            try
            {
                var options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(15)
                };

                bool Filter(SocketMessage message, int index)
                {
                    return message.Author.Id == invoker.Id && message.Channel.Id == channel.Id;
                }

                await _collector.RunSessionAsync(session, Filter, options);
            }
            catch (Exception e)
            {
                await channel.CatchAsync(e);
            }

            invoker.CanGamble = true;
        }
    }
}
