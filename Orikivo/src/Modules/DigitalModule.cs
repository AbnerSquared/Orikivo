using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Desync;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    [Name("Digital")]
    [Summary("Commands that relate to the digital client.")]
    public class DigitalModule : OriModuleBase<OriCommandContext>
    {
        [Command("merits")]
        [Summary("View a summary about your current **Merit** progression.")]
        [RequireUser]
        public async Task GetMeritsAsync(MeritGroup? group = null)
        {
            if (group.HasValue)
                await Context.Channel.SendMessageAsync(Context.Account, MeritHandler.ViewCategory(Context.Account, group.Value));
            else
                await Context.Channel.SendMessageAsync(Context.Account, MeritHandler.ViewDefault(Context.Account));
        }

        [RequireUser]
        [Command("notifications")]
        public async Task ViewNotificationsAsync(int page = 1)
        {
            // TODO: Paginator.
            await Context.Channel.SendMessageAsync(Context.Account.Notifier.Display());
        }

        [Command("stats")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetAllStatsAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Container.TryGetUser(user.Id, out User account);

            string stats = string.Join("\n", account.Stats.Select(s => $"{StatHandler.GetNameOrDefault(s.Key)}: {s.Value}"));

            if (Context.User.Id != user.Id)
                stats = $"> **Stats** ({user.Username})\n" + stats;

            await Context.Channel.SendMessageAsync(Context.Account, stats == "" ? "No stats." : stats);
        }
    }
}