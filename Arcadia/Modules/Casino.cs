using Discord.Commands;
using Orikivo;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia
{

    [Name("Casino")]
    [Summary("Come and gamble your life away.")]
    public class Casino : OriModuleBase<OriCommandContext>
    {
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("balance"), Alias("money", "bal")]
        public async Task GetMoneyAsync()
        {
            StringBuilder values = new StringBuilder();

            values.AppendLine($"**Balance**: 💸 **{Context.Account.Balance.ToString("##,0.###")}**");
            values.AppendLine($"**Tokens**: 🏷️ **{Context.Account.TokenBalance.ToString("##,0.###")}**");
            values.AppendLine($"**Debt**: 📃 **{Context.Account.Debt.ToString("##,0.###")}**");

            await Context.Channel.SendMessageAsync(values.ToString());
        }

        [Group("gimi")]
        public class GimiGroup : OriModuleBase<OriCommandContext>
        {
            [Command("")]
            //[Cooldown(5)]
            [RequireUser]
            [Summary("A **CasinoActivity** that randomly offers a reward value (if you're lucky enough).")]
            public async Task GimiAsync()
            {
                Gimi gimi = new Gimi(Context.Account);
                GimiResult result = gimi.Next();

                await Context.Channel.SendMessageAsync(result.ApplyAndDisplay(Context.Account));
            }

            [Command("risk"), Priority(0)]
            [RequireUser]
            public async Task GetGimiRiskAsync()
            {
                //await Context.Channel.SendMessageAsync($"> Your **Risk** is currently set to **{Context.Account.Gimi.Risk}**%.");
            }

            [Command("risk"), Priority(1)]
            [RequireUser]
            public async Task SetGimiRiskAsync(int risk)
            {
                //Context.Account.Gimi.SetRisk(risk);
                //await Context.Channel.SendMessageAsync($"> Your **Risk** has been set to **{Context.Account.Gimi.Risk}**%.");
            }

            // find names that sound and work better.
            [RequireUser]
            [Command("range"), Priority(0)]
            public async Task SetGimiRangeAsync()
            {
                //await Context.Channel.SendMessageAsync($"> Your **Earn** is currently set to {Context.Account.Gimi.Earn}.");
            }

            [RequireUser]
            [Command("range"), Priority(1)]
            public async Task SetGimiRangeAsync(int earn)
            {
                //Context.Account.Gimi.SetEarn(earn);
                //await Context.Channel.SendMessageAsync($"> Your **Earn** has been set to **{Context.Account.Gimi.Earn}**.");
            }
        }
    }
}