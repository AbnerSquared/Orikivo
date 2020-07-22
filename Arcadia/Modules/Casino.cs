using Discord.Commands;
using Orikivo;
using System.Threading.Tasks;

namespace Arcadia
{
    [Name("Casino")]
    [Summary("Come and gamble your life away.")]
    public class Casino : OriModuleBase<ArcadeContext>
    {
        [Command("gimi")]
        [RequireUser]
        [Summary("An activity that randomly offers a reward value (if you're lucky enough).")]
        public async Task GimiAsync()
        {
            var gimi = new Gimi();
            GimiResult result = gimi.Next();

            await Context.Channel.SendMessageAsync(result.ApplyAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("doubler"), Alias("double", "dbl")]
        [Summary("A **Casino** activity that allows you to attempt to make an astonishing return.")]
        public async Task DoublerAsync(long wager, int expectedTick = 1, TickWinMethod method = TickWinMethod.Below)
        {
            if (wager < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (wager == 0)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You need to specify a positive amount of **Chips** to bet.");
                return;
            }

            if (wager > (long)Context.Account.ChipBalance)
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

            await Context.Channel.SendMessageAsync(result.ApplyAndDisplay(Context.Account));
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
                await Context.Channel.SendMessageAsync($"> ⚠️ You need to specify a positive amount of **Orite** to convert.");
                return;
            }

            if (amount > (long)Context.Account.Balance)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You don't have enough **Orite** to convert this amount.");
                return;
            }

            var chips = (ulong) MoneyConvert.GetChips(amount);

            Context.Account.Take(amount);
            Context.Account.ChipBalance += chips;
            await Context.Channel.SendMessageAsync($"> You have traded in **💸 {amount.ToString("##,0")}** in exchange for **🧩 {chips.ToString("##,0")}**.");

        }
    }
}