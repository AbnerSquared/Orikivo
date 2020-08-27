using System;
using System.Text;
using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;
using Arcadia.Casino;
using Orikivo.Framework;

namespace Arcadia.Modules
{
    [Name("Casino")]
    [Summary("Come and gamble your life away.")]
    public class Casino : OriModuleBase<ArcadeContext>
    {
        [Command("castingtest")]
        public async Task TestAsync()
        {
            Func<ArcadeUser, object, string> criteria = (u, r) => "SUCCESS";
            ReplyWriter w = criteria.Invoke;

            string result = w(Context.Account, null);
            Logger.Debug(result);
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

        [RequireItem("au_gimi")]
        [Command("autogimi")] // You need to find a better way to process automation in the background without taking up too many threads
        public async Task AutoGimiAsync(int times)
        {
            if (!ItemHelper.HasItem(Context.Account, Items.AutomatonGimi))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are missing the **Automaton: Gimi** component in order to execute this method."));
                return;
            }

            if (!Context.Account.CanAutoGimi)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You have already started an automated run."));
                return;
            }

            await HandleAutoGimiAsync(times).ConfigureAwait(false);
        }

        private async Task HandleAutoGimiAsync(int times)
        {
            Context.Account.CanAutoGimi = false;
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
            Context.Account.CanAutoGimi = true;
        }

        [RequireUser]
        [Command("doubler"), Alias("double", "dbl")]
        [Summary("A **Casino** activity that allows you to attempt to make an astonishing return.")]
        public async Task DoublerAsync(long wager = 0, int expectedTick = 1, TickWinMethod method = TickWinMethod.Below)
        {
            if (wager < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (wager == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Chips** to bet."
                await Context.Channel.SendMessageAsync(CommandDetails.WriteTick());
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

            var tick = new Tick(expectedTick, method);

            TickResult result = tick.Next(wager);

            await Context.Channel.SendMessageAsync(Context.Account, result.ApplyAndDisplay(Context.Account));
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
                await Context.Channel.SendMessageAsync(CommandDetails.WriteGetChips());
                return;
            }

            if (amount > Context.Account.Balance)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You don't have enough **Orite** to convert this amount.");
                return;
            }

            var chips = MoneyConvert.GetChips(amount);

            Context.Account.Take(amount, false);
            Context.Account.ChipBalance += chips;
            await Context.Channel.SendMessageAsync($"> You have traded in **💸 {amount:##,0}** in exchange for **🧩 {chips:##,0}**.");

        }
    }
}