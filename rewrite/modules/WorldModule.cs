using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    [Name("World")]
    //[Subtitle("ok woomer")]
    [Summary("Commands that relate to the world and digital client.")]
    public class WorldModule : OriModuleBase<OriCommandContext>
    {
        [Command("merits")]
        [Summary("View a collection of merits")]
        [RequireUser]
        public async Task GetMeritsAsync(MeritGroup? group = null)
        {
            await Context.Channel.SendMessageAsync(WorldService.GetMeritPanel(Context.Account, group));
        }

        [Command("cry")]
        [RequireUser]
        [Summary("cry.")]
        public async Task CryAsync()
        {
            Context.Account.UpdateStat("times_cried", 1);
            await Context.Channel.SendMessageAsync("You have cried.");
        }

        [Command("stats")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetAllStatsAsync()
        {
            string stats = string.Join("\n", Context.Account.Stats.Select(s => $"{WorldService.GetNameOrDefault(s.Key)}: {s.Value}"));
            await Context.Channel.SendMessageAsync(stats == "" ? "No stats." : stats);
        }

    }
}