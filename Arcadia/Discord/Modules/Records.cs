using System.Threading.Tasks;
using Arcadia.Services;
using Discord.Commands;
using Orikivo;

namespace Arcadia.Modules
{
    [Icon("🖨️")]
    [Name("Records")]
    [Summary("View information about unlocked information and extra statistics.")]
    public class Records : ArcadeModule
    {
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("profile"), Alias("account", "acc", "pf", "user")]
        [Summary("View a profile.")]
        public async Task ViewProfileAsync(ArcadeUser user = null)
        {
            user ??= Context.Account;
            await Context.Channel.SendMessageAsync(ProfileViewer.View(user, Context));
        }

        [RequireUser]
        [Command("boosters"), Alias("rates")]
        [Summary("View all of your currently equipped boosters.")]
        public async Task ViewBoostersAsync()
        {
            await Context.Channel.SendMessageAsync(BoostViewer.Write(Context.Account));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("memo")]
        [Summary("View a research memo on the specified **Item**.")]
        public async Task ViewMemoAsync(Item item)
        {
            string result = ResearchHelper.ViewMemo(Context.Account, item);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("memos")]
        [Summary("View memos about previous research.")]
        public async Task ViewMemosAsync(int page = 1)
        {
            string result = ResearchHelper.ViewMemos(Context.Account, --page);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("recipe"), Priority(0)]
        [Summary("View the crafting requirements for a specific **Recipe**.")]
        public async Task ViewRecipeAsync([Name("recipe_id")]Recipe recipe)
        {
            await Context.Channel.SendMessageAsync(SRecipeViewer.ViewRecipe(Context.Account, recipe));
        }

        [RequireUser]
        [Command("recipes"), Priority(0)]
        [Summary("View all of your currently known recipes.")]
        public async Task ViewRecipesAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(SRecipeViewer.View(Context.Account, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("recipes"), Priority(1)]
        [Summary("View your known recipe variations for a specific **Item**.")]
        public async Task ViewRecipeAsync([Name("item_id")]Item item, int page = 1)
        {
            await Context.Channel.SendMessageAsync(SRecipeViewer.ViewItemRecipes(Context.Account, item, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("catalog"), Alias("items")]
        [Summary("View all of the items you have seen or known about.")]
        public async Task ViewCatalogAsync(string query = null, int page = 1)
        {
            await Context.Channel.SendMessageAsync(CatalogViewer.View(Context.Account, query, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        // [Command("catalogsearch")]
        [Summary("Search through the item catalog to look for a specific **Item**.")]
        public async Task CatalogSearchAsync([Remainder]string input)
        {
            if (!Check.NotNull(input))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You must specify a reference for the catalog to use."));
                return;
            }

            await Context.Channel.SendMessageAsync(CatalogViewer.Search(Context.Account, input));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("item")]
        [Summary("Provides details about the specified **Item**, if it has been previously discovered.")]
        public async Task ViewItemAsync([Summary("The **Item** to view more information about.")]Item item)
        {
            CatalogStatus status = CatalogHelper.GetCatalogStatus(Context.Account, item);

            if (status == CatalogStatus.Unknown && Context.Account.Items.Exists(x => x.Id == item.Id))
                CatalogHelper.SetCatalogStatus(Context.Account, item, CatalogStatus.Known);

            await Context.Channel.SendMessageAsync(CatalogViewer.ViewItem(item, CatalogHelper.GetCatalogStatus(Context.Account, item)));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("statsof")]
        [Summary("View a collection of stats in the specified group.")]
        public async Task GetGroupStatsAsync(string query, int page = 1)
        {
            await Context.Channel.SendMessageAsync(StatHelper.WriteFor(Context.Account, query, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("statsfor")]
        [Summary("View another user's collection of stats.")]
        public async Task GetStatsForAsync(ArcadeUser user, int page = 1)
        {
            await Context.Channel.SendMessageAsync(StatHelper.Write(user, false, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("stats")]
        [Summary("View your current collection of stats.")]
        public async Task GetStatsAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(StatHelper.Write(Context.Account, page: --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("leaderboard"), Alias("top"), Priority(0)]
        [Summary("Filters a custom leaderboard based on a specified **Stat**.")]
        public async Task GetLeaderboardAsync([Name("stat_id")]string statId, LeaderboardSort sort = LeaderboardSort.Most, int page = 1)
        {
            var board = new Leaderboard(statId, sort);
            string result = board.Write(Context.Account, Context.Data.Users.Values.Values, --page);

            await Context.Channel.SendMessageAsync(result);
        }

        // TODO: Implement enum value listings
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("leaderboard"), Alias("top"), Priority(1)]
        [Summary("View the current pioneers of a specific category.")]
        public async Task GetLeaderboardAsync(LeaderboardQuery flag = LeaderboardQuery.Default, LeaderboardSort sort = LeaderboardSort.Most, int page = 1)
        {
            if (flag == LeaderboardQuery.Custom)
                flag = LeaderboardQuery.Default;

            var board = new Leaderboard(flag, sort);
            string result = board.Write(Context.Account, Context.Data.Users.Values.Values, --page);

            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("merit")]
        [Summary("View information about a **Merit**.")]
        public async Task ViewMeritAsync(Merit merit)
        {
            if (!MeritHelper.HasUnlocked(Context.Account, merit) && merit.Hidden)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are not authorized to view this merit."));
                return;
            }

            await Context.Channel.SendMessageAsync(MeritHelper.ViewMerit(merit, Context.Account));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("merits")]
        [Summary("Search and view all of your known merits.")]
        public async Task ViewMeritsAsync(string query = null, int page = 1)
        {
            await Context.Channel.SendMessageAsync(MeritHelper.View(Context.Account, query, --page));
        }

        [RequireUser]
        [Command("stat")]
        [Summary("View details about a single **Stat**.")]
        public async Task ViewStatAsync([Name("stat_id")]string id)
        {
            await Context.Channel.SendMessageAsync(StatHelper.ViewDetails(Context.Account, id, Context.Data.Users.Values.Values));
        }
    }
}
