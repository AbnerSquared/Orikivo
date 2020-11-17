using System;
using System.Threading.Tasks;
using Arcadia.Casino;
using Discord.Addons.Collectors;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Text;

namespace Arcadia.Modules
{
    public class CasinoService
    {
        private readonly MessageCollector _collector;
        private readonly LocaleProvider _locale;
        public CasinoService(DiscordSocketClient client, LocaleProvider locale)
        {
            _collector = new MessageCollector(client);
            _locale = locale;
        }

        public async Task RunBlackJackAsync(ArcadeUser invoker, ISocketMessageChannel channel, Wager wager)
        {
            if (invoker.IsInSession)
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

            invoker.IsInSession = true;
            var session = new BlackJackSession(invoker, channel, wager.Value, _locale);

            try
            {
                var options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(20)
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

            invoker.IsInSession = false;
        }
    }
}
