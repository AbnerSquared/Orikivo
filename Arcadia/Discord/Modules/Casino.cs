using System;
using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;
using Arcadia.Casino;
using Arcadia.Multiplayer;

namespace Arcadia.Modules
{
    [Icon("🎰")]
    [Name("Casino")]
    [Summary("Come and gamble your life away.")]
    public class Casino : BaseModule<ArcadeContext>
    {
        private readonly CasinoService _service;
        public Casino(CasinoService service)
        {
            _service = service;
        }

        [RequireUser]
        [Command("blackjack")]
        [Summary("A game of 21.")]
        public async Task BlackJackAsync(Wager wager)
        {
            await _service.RunBlackJackAsync(Context.Account, Context.Channel, wager).ConfigureAwait(false);
        }

        [RequireUser]
        [Command("roulette")]
        [Summary("A Casino classic. Choose your style of bet and go wild.")]
        public async Task RouletteAsync(RouletteBetMode mode, long wager)
        {
            if (wager < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (wager == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Chips** to bet."
                await Context.Channel.SendMessageAsync($"> ⚠️ You need to specify a positive amount of **Chips** to bet.");
                return;
            }

            if (wager > Context.Account.ChipBalance)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You don't have enough **Chips** to bet with.");
                return;
            }

            if (wager > Roulette.MaxWager)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ The maximum wager for **Roulette** is {Icons.Chips} **{Roulette.MaxWager:##,0}**.");
                return;
            }

            RouletteResult result = Roulette.Next(Context.Account, mode, wager);
            Message message = result.ApplyAndDisplay(Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, message).ConfigureAwait(false);
        }

        [Command("gimi")]
        [RequireUser]
        [Summary("An activity that randomly offers a reward value (if you're lucky enough).")]
        public async Task GimiAsync()
        {
            var gimi = new Gimi();
            GimiResult result = gimi.Next();

            Message message = result.ApplyAndDisplay(Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, message).ConfigureAwait(false);
        }

        [Session]
        [RequireItem("au_gimi")]
        [Command("autogimi")] // You need to find a better way to process automation in the background without taking up too many threads
        public async Task AutoGimiAsync(int times)
        {
            if (!ItemHelper.HasItem(Context.Account, Ids.Items.AutomatonGimi))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are missing the **Automaton: Gimi** component in order to execute this method."));
                return;
            }

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
        [Summary("A **Casino** activity that allows you to attempt to make an astonishing return.")]
        public async Task DoublerAsync(long wager = 0, int expectedTick = 1, DoublerWinMethod method = DoublerWinMethod.Below)
        {
            if (wager < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (wager == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Chips** to bet."
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteTick());
                return;
            }

            if (wager > Context.Account.ChipBalance)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You don't have enough **Chips** to bet with.");
                return;
            }

            if (expectedTick <= 0)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You have specified an invalid tick. Try something that's above **0**.");
                return;
            }

            var tick = new Doubler(expectedTick, method);

            DoublerResult result = tick.Next(wager, Context.Account.GetVar(Stats.Doubler.CurrentLossStreak));
            Message message = result.ApplyAndDisplay(Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, message);
        }

        // TODO: Implement a proper cashing out system.
        /*
        [RequireUser]
        [Command("cashout")]
        [Summary("Convert your chips back into money.")]
        public async Task CashOutAsync(long amount = 0)
        {

        }
        */

        [RequireUser]
        [Command("getchips")]
        [Summary("Convert some of your money into chips.")]
        public async Task GetChipsAsync(long amount = 0)
        {
            if (amount < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (amount == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Orite** to convert.
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteGetChips());
                return;
            }

            if (amount > Context.Account.Balance)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You don't have enough **Orite** to convert this amount.");
                return;
            }

            var chips = MoneyConvert.ToChips(amount);

            Context.Account.Take(amount);
            Context.Account.ChipBalance += chips;
            await Context.Channel.SendMessageAsync($"> You have traded in **💸 {amount:##,0}** in exchange for **🧩 {chips:##,0}**.");
        }
    }
}