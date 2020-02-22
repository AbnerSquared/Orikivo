using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Unstable;
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
        public async Task GetAllStatsAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Container.TryGetUser(user.Id, out User account);

            string stats = string.Join("\n", account.Stats.Select(s => $"{WorldService.GetNameOrDefault(s.Key)}: {s.Value}"));

            if (Context.User.Id != user.Id)
                stats = $"> **Stats** ({user.Username})\n" + stats;

            await Context.Channel.SendMessageAsync(stats == "" ? "No stats." : stats);
        }

    }
}