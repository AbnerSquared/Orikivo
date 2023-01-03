using System;
using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;
using Arcadia.Casino;
using Arcadia.Multiplayer;
using Orikivo.Text;
using Arcadia.Commands;

namespace Arcadia.Modules
{
    [Icon("🎰")]
    [Name("Casino")]
    [Summary("Come and gamble your life away.")]
    public class Casino : ArcadeModule
    {
        private readonly CasinoService _service;
        private readonly LocaleProvider _locale;

        public Casino(CasinoService service, LocaleProvider locale)
        {
            _service = service;
            _locale = locale;
        }

        [RequireUser]
        [Command("blackjack")]
        [Summary("A casino game where your goal is to reach as close to 21 as possible without going over.")]
        public async Task BlackJackAsync(Wager wager)
        {
            await _service.RunBlackJackAsync(Context.Account, Context.Channel, wager).ConfigureAwait(false);
        }

        [RequireUser]
        [Command("roulette")]
        [Summary("A casino classic. Choose your betting style and hope for the best.")]
        public async Task RouletteAsync(RouletteBetMode mode, Wager wager)
        {
            if (wager.Value < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ {_locale.GetValue("warning_negative_wager", Context.Account.Config.Language)}\n> *\"{_locale.GetValue("warning_negative_wager_subtitle", Context.Account.Config.Language)}\"*");
                return;
            }

            if (wager.Value == 0)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_empty_wager", Context.Account.Config.Language)));
                return;
            }

            if (wager.Value > Context.Account.ChipBalance)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_wager", Context.Account.Config.Language)));
                return;
            }

            if (wager.Value > Roulette.MaxWager)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_wager_cap", Context.Account.Config.Language, Format.Bold("Roulette"), CurrencyHelper.WriteCost(Roulette.MaxWager, CurrencyType.Token))));
                return;
            }

            RouletteResult result = Roulette.Next(Context.Account, mode, wager.Value);
            Message message = result.ApplyAndDisplay(Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, message).ConfigureAwait(false);
        }

        [RequireUser]
        [Command("gimi")]
        [Summary("An casino-like activity that randomly provides curses or blessings.")]
        public async Task GimiAsync()
        {
            var gimi = new Gimi();
            GimiResult result = gimi.Next();

            Message message = result.ApplyAndDisplay(Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, message).ConfigureAwait(false);
        }

        // NOTE: If this proves to be too much for multi-threading, remove this functionality entirely
        [Session]
        [RequireItem("au_gimi")]
        [Command("autogimi")] // TODO: Find a better way to process automation in the background without taking up too many threads
        [Summary("Begins an automated run of **Gimi**, up to 100 runs.")]
        public async Task AutoGimiAsync(int times)
        {
            await HandleAutoGimiAsync(times).ConfigureAwait(false);
        }

        private async Task HandleAutoGimiAsync(int times)
        {
            Context.Account.IsInSession = true;
            times = times > 100 ? 100 : times <= 0 ? 1 : times;

            long result = 0;

            TimeSpan duration = TimeSpan.FromSeconds(times - 1);
            Discord.IUserMessage reference = await Context.Channel.SendMessageAsync($"> Gathering results in {Format.Counter(duration.TotalSeconds)}...");
            var gimi = new Gimi(true);

            for (int i = 0; i < times; i++)
            {
                GimiResult innerResult = gimi.Next();

                // this should be easier on the mem
                innerResult.Apply(Context.Account);
                result += innerResult.IsSuccess ? innerResult.Reward : -innerResult.Reward;

                if (times > 1)
                    await Task.Delay(1000);
            }

            await reference.ModifyAsync($"> 〽️ **The results are in!**\n> **Net Outcome**: **{result:##,0}**").ConfigureAwait(false);
        }

        [RequireUser]
        [Command("doubler"), Alias("double", "dbl")]
        [Summary("A casino game mode that grants you a chance at large returns by guessing the surviving tick.")]
        public async Task DoublerAsync(
            [Summary("The amount of **Chips** to bet for this round.")]Wager wager = null,
            [Summary("The tick that you think the doubler will stop at.")]int tick = 1,
            [Summary("The method to use for this round.")]DoublerWinMethod method = DoublerWinMethod.Below)
        {
            if (wager == null)
            {
                // $"> ⚠️ You need to specify a positive amount of **Chips** to bet."
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteTick());
                return;
            }

            if (wager.Value < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ {_locale.GetValue("warning_negative_wager", Context.Account.Config.Language)}\n> *\"{_locale.GetValue("warning_negative_wager_subtitle", Context.Account.Config.Language)}\"*");
                return;
            }

            if (wager.Value == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Chips** to bet."
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteTick());
                return;
            }

            if (wager.Value > Context.Account.ChipBalance)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_wager", Context.Account.Config.Language)));
                return;
            }

            if (tick <= 0)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_invalid_tick", Context.Account.Config.Language)));
                return;
            }

            var doubler = new Doubler(tick, method);

            DoublerResult result = doubler.Next(wager.Value, Context.Account.GetVar(Stats.Doubler.CurrentLossStreak));
            Message message = result.ApplyAndDisplay(Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, message);
        }

        [RequireUser]
        [Command("getchips")]
        [Summary("Convert some of your **Orite** into **Chips** for use in casino activities.")]
        public async Task GetChipsAsync(Wager amount = null)
        {
            if (amount == null)
            {
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteGetChips());
                return;
            }

            if (amount.Value < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ {_locale.GetValue("warning_negative_wager", Context.Account.Config.Language)}\n> *\"{_locale.GetValue("warning_negative_wager_subtitle", Context.Account.Config.Language)}\"*");
                return;
            }

            if (amount.Value > Context.Account.Balance)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_missing_convert", Context.Account.Config.Language, Format.Bold("Orite"))));
                return;
            }

            var chips = MoneyConvert.ToChips(amount.Value);

            Context.Account.Take(amount.Value);
            Context.Account.ChipBalance += chips;
            await Context.Channel.SendMessageAsync($"> {_locale.GetValue("currency_convert_success", Context.Account.Config.Language, CurrencyHelper.WriteCost(amount.Value, CurrencyType.Cash), CurrencyHelper.WriteCost(chips, CurrencyType.Token))}");
        }
    }
}
