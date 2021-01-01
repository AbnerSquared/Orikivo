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
                await channel.SendMessageAsync($"> 👁️ {_locale.GetValue("warning_negative_wager", invoker.Config.Language)}\n> *\"{_locale.GetValue("warning_negative_wager_subtitle", invoker.Config.Language)}\"*");
                return;
            }

            if (wager.Value == 0)
            {
                await channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_empty_wager", invoker.Config.Language)));
                return;
            }

            if (wager.Value > invoker.ChipBalance)
            {
                await channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_wager", invoker.Config.Language)));
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
