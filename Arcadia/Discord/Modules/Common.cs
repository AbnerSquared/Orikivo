using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Formatters;
using Arcadia.Services;
using Arcadia.Graphics;
using DiscordBoats;
using Microsoft.Extensions.Configuration;
using Orikivo.Text;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    // - Card Customization

    // TODO: Transfer everything to CommonService
    [Icon("🧮")]
    [Name("Common")]
    [Summary("Generic commands that are commonly used.")]
    public class Common : BaseModule<ArcadeContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfigurationRoot _config;

        public Common(DiscordSocketClient client, IConfigurationRoot config)
        {
            _client = client;
            _config = config;
        }

        [RequireUser]
        [Command("submit")]
        [Summary("Submit your challenge set for completion.")]
        public async Task SubmitChallengesAsync()
        {
            string result = ChallengeHelper.Submit(Context.Account);
            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser]
        [Command("challenges")]
        [Summary("View your current challenge set.")]
        public async Task ViewChallengesAsync()
        {
            string result = ChallengeHelper.View(Context.Account);
            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser]
        [Command("memos")]
        [Summary("View memos about previous research.")]
        public async Task ViewMemosAsync(int page = 1)
        {
            string result = ResearchHelper.ViewMemos(Context.Account, --page);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("memo")]
        [Summary("View a research memo.")]
        public async Task ViewMemoAsync(Item item)
        {
            string result = ResearchHelper.ViewMemo(Context.Account, item);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("researchinfo"), Alias("rinfo")]
        [Summary("View details about the research progress of an item.")]
        public async Task ReadResearchAsync(Item item)
        {
            string result = ResearchHelper.ViewResearch(Context.Account, item);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("research"), Priority(0)]
        [Summary("View the current progress of your research.")]
        public async Task ViewResearchAsync()
        {
            string result = ResearchHelper.ViewProgress(Context.Account);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("research"), Priority(1)]
        public async Task ResearchAsync(Item item)
        {
            string result = ResearchHelper.ResearchItem(Context.Account, item);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [Command("guides")]
        [Summary("Learn how to use **Orikivo Arcade** with this collection of guides.")]
        public async Task ViewGuidesAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(GuideViewer.View(--page));
        }

        [Command("guide")]
        [Summary("Read and view the contents of the specified guide.")]
        public async Task ReadGuideAsync([Name("guide_id")]string id, int page = 1)
        {
            await Context.Channel.SendMessageAsync(GuideViewer.ViewGuide(id, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("recipes")]
        [Summary("View all of your currently known recipes.")]
        public async Task ViewRecipesAsync()
        {
            await Context.Channel.SendMessageAsync(RecipeViewer.View(Context.Account));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("recipe")]
        [Summary("View information about a specific recipe.")]
        public async Task ViewRecipeAsync([Name("recipe_id")]Recipe recipe)
        {
            /*
            if (!ItemHelper.RecipeExists(recipeId))
            {
                await Context.Channel.WarnAsync("Could not find the specified recipe.");
                return;
            }*/

            await Context.Channel.SendMessageAsync(RecipeViewer.ViewRecipeInfo(Context.Account, recipe));
        }

        [RequireUser]
        [Command("craft")]
        [Summary("Attempt to craft an **Item** using the specified **Recipe**.")]
        public async Task CraftAsync([Name("recipe_id")]Recipe recipe)
        {
            /*
            if (!ItemHelper.RecipeExists(recipeId))
            {
                await Context.Channel.WarnAsync("Could not find the specified recipe.");
                return;
            }*/

            if (!CraftHelper.CanCraft(Context.Account, recipe))
            {
                var notice = new StringBuilder();

                notice.AppendLine(Format.Warning("You are unable to craft this recipe."));
                notice.AppendLine("\n> **Missing Components**");

                foreach ((string itemId, int amount) in CraftHelper.GetMissingFromRecipe(Context.Account, recipe))
                {
                    notice.AppendLine(RecipeViewer.WriteRecipeComponent(itemId, amount));
                }

                await Context.Channel.SendMessageAsync(notice.ToString());
                return;
            }


            if (CraftHelper.Craft(Context.Account, recipe))
            {
                var result = new StringBuilder();

                result.AppendLine($"> 📑 **{ItemHelper.NameOf(recipe.Result.ItemId)}**{(recipe.Result.Amount > 1 ? $" (x**{recipe.Result.Amount:##,0}**)" : "")}");
                result.AppendLine("> Successfully crafted an item!");

                result.AppendLine("\n> **Losses**");

                foreach ((string itemId, int amount) in recipe.Components)
                {
                    string icon = ItemHelper.IconOf(itemId);

                    if (!Check.NotNull(icon))
                        icon = "•";

                    result.AppendLine($"{icon} **{ItemHelper.NameOf(itemId)}**{(amount > 1 ? $" (x**{amount:##,0}**)" : "")}");
                }

                await Context.Channel.SendMessageAsync(result.ToString());
                return;
            }

            await Context.Channel.SendMessageAsync(Format.Warning("An unknown error has occured when crafting this recipe."));

        }

        [RequireUser]
        [Command("boosters"), Alias("rates")]
        [Summary("View all of your currently equipped boosters.")]
        public async Task ViewBoostersAsync()
        {
            await Context.Channel.SendMessageAsync(BoostViewer.Write(Context.Account));
        }

        [RequireUser]
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

        [RequireUser]
        [Command("cooldowns"), Alias("cooldown", "expiry")]
        [Summary("View all currently active cooldowns and expirations.")]
        public async Task ViewCooldownsAsync()
        {
            await Context.Channel.SendMessageAsync(CooldownHelper.ViewAllTimers(Context.Account));
        }

        [RequireUser]
        [Command("claim")]
        [Summary("Attempt to claim the specified **Merit**.")]
        public async Task ClaimAsync(string meritId = null)
        {
            await Context.Channel.SendMessageAsync(MeritHelper.Claim(Context.Account, meritId));
        }

        [RequireUser]
        [Command("stat")]
        [Summary("View details about a single **Stat**.")]
        public async Task ViewStatAsync([Name("stat_id")]string id)
        {
            await Context.Channel.SendMessageAsync(Var.ViewDetails(Context.Account, id, Context.Data.Users.Values.Values));
        }

        [RequireUser]
        [Command("level"), Alias("exp", "ascent", "lv", "xp")]
        [Summary("View your current level.")]
        public async Task ViewLevelAsync()
        {
            await Context.Channel.SendMessageAsync(LevelViewer.View(Context.Account));
        }

        [RequireUser]
        [Command("merits")]
        [Summary("Search and view all of your known merits.")]
        public async Task ViewMeritsAsync(string query = null, int page = 1)
        {
            await Context.Channel.SendMessageAsync(MeritHelper.View(Context.Account, query, --page));
        }

        [RequireUser]
        [Command("quests"), Alias("missions", "tasks"), Priority(0)]
        [Summary("View all of your currently assigned quests.")]
        public async Task ViewQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.View(Context.Account));
        }

        [RequireUser]
        [Command("quests"), Alias("missions", "tasks"), Priority(1)]
        [Summary("View the currently assigned objective on the specified slot.")]
        public async Task ViewQuestAsync(int slot)
        {
            await Context.Channel.SendMessageAsync(QuestHelper.ViewSlot(Context.Account, --slot));
        }

        [RequireUser]
        [Command("assign")]
        [Summary("Assign a new set of quests.")]
        public async Task AssignQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.AssignAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("toss")]
        [Summary("Toss the specified quest you are currently working on.")]
        public async Task TossQuestAsync(int slot)
        {
            await Context.Channel.SendMessageAsync(QuestHelper.TossSlot(Context.Account, --slot));
        }

        [RequireUser]
        [Command("complete")]
        [Summary("Claim the rewards from all of your completed quests.")]
        public async Task CompleteQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.CompleteAndDisplay(Context.Account));
        }

        /*
        [Group("quests"), Alias("missions", "tasks")]
        public class Quests : BaseModule<ArcadeContext>
        {
            [RequireUser]
            [Command("assign")]
            [Summary("Assign a new set of quests.")]
            public async Task AssignQuestsAsync()
            {
                await Context.Channel.SendMessageAsync(QuestHelper.AssignAndDisplay(Context.Account));
            }

            [RequireUser]
            [Command("toss")]
            [Summary("Toss the specified quest you are currently working on.")]
            public async Task TossQuestAsync(int slot)
            {
                await Context.Channel.SendMessageAsync(QuestHelper.TossSlot(Context.Account, --slot));
            }

            [RequireUser]
            [Command("complete")]
            [Summary("Claim the rewards from all of your completed quests.")]
            public async Task CompleteQuestsAsync()
            {
                await Context.Channel.SendMessageAsync(QuestHelper.CompleteAndDisplay(Context.Account));
            }
        }
        */

        [RequireUser]
        [Command("vote")]
        public async Task VoteAsync()
        {
            if (string.IsNullOrWhiteSpace(_config["token_discord_boats"]))
                throw new Exception("Expected valid API token for Discord Boats");

            using var boatClient = new BoatClient(_client.CurrentUser.Id, _config["token_discord_boats"]);
            VoteResultFlag result = VoteService.Next(boatClient, Context.Account);
            await Context.Channel.SendMessageAsync(VoteService.ApplyAndDisplay(Context.Account, result));
        }

        [RequireUser]
        [Command("cashout")]
        [Summary("Cash out **Tokens** for **Orite**.")]
        public async Task CashOutAsync(long amount = 0)
        {
            if (amount < 0)
            {
                await Context.Channel.SendMessageAsync($"> 👁️ You can't specify a **negative** value.\n> *\"I know what you were trying to do.\"*");
                return;
            }

            if (amount == 0)
            {
                // $"> ⚠️ You need to specify a positive amount of **Orite** to convert.
                await Context.Channel.SendMessageAsync(CommandDetailsViewer.WriteCashOut());
                return;
            }

            if (amount > Context.Account.TokenBalance)
            {
                await Context.Channel.SendMessageAsync($"> ⚠️ You don't have enough **Tokens** to convert this amount.");
                return;
            }

            long money = MoneyConvert.TokensToMoney(amount);

            Context.Account.Take(amount, CurrencyType.Tokens);
            Context.Account.Give(money, CurrencyType.Money);
            await Context.Channel.SendMessageAsync($"> You have traded in **{Icons.Tokens} {amount:##,0}** in exchange for **{Icons.Balance} {money:##,0}**.");
        }

        [RequireUser]
        [Command("daily")]
        public async Task GetDailyAsync()
        {
            DailyResultFlag result = DailyService.Next(Context.Account);
            await Context.Channel.SendMessageAsync(DailyService.ApplyAndDisplay(Context.Account, result));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("catalog"), Alias("items")]
        [Summary("View all of the items you have seen or known about.")]
        public async Task ViewCatalogAsync(string query = null, int page = 1)
        {
            await Context.Channel.SendMessageAsync(CatalogViewer.View(Context.Account, query, --page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("catalogsearch")]
        [Summary("Search through your item catalog to find a specific item.")]
        public async Task CatalogSearchAsync([Remainder] string input)
        {
            if (!Check.NotNull(input))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You must specify a reference for the catalog to use."));
                return;
            }

            await Context.Channel.SendMessageAsync(CatalogViewer.Search(Context.Account, input));
        }

        [RequireUser]
        [Command("use")]
        [Summary("Uses the specified **Item** by its internal or unique ID.")]
        public async Task UseItemAsync(string dataId, [Remainder]string input = null)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);

            if (data == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find a data reference."));
                return;
            }

            UsageResult result = ItemHelper.UseItem(Context.Account, data, input);

            if (result.Message != null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, result.Message);
                return;
            }

            await Context.Channel.SendMessageAsync(Format.Warning($"You have used **{ItemHelper.NameOf(data.Id)}**."));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("item")]
        [Summary("Provides details about the specified **Item**, if it has been previously discovered.")]
        public async Task ViewItemAsync(Item item)
        {
            CatalogStatus status = CatalogHelper.GetCatalogStatus(Context.Account, item);

            if (status == CatalogStatus.Unknown && Context.Account.Items.Exists(x => x.Id == item.Id))
                CatalogHelper.SetCatalogStatus(Context.Account, item, CatalogStatus.Known);

            await Context.Channel.SendMessageAsync(CatalogViewer.ViewItem(item, CatalogHelper.GetCatalogStatus(Context.Account, item)));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inspectat")]
        [Summary("Inspect at specific **Item** slot in your inventory.")]
        public async Task ViewInventorySlotAsync([Name("slot_index")]int slot)
        {
            if (Context.Account.Items.Count == 0)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You do not have any items in your inventory."));
                return;
            }

            slot--;

            Math.Clamp(slot, 0, Context.Account.Items.Count - 1);

            ItemData data = Context.Account.Items[slot];

            await Context.Channel.SendMessageAsync(CatalogViewer.InspectItem(Context, Context.Account, data, slot));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inspect")]
        [Summary("Inspect a specific **Item** in your inventory using a data reference.")]
        public async Task InspectInInventoryAsync([Name("data_id")]string dataId)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);
            int slot = Context.Account.Items.IndexOf(data);

            if (data == null)
            {
                if (ItemHelper.Exists(dataId))
                {
                    await Context.Channel.SendMessageAsync(Format.Warning("You do not own this item."));
                }

                await Context.Channel.SendMessageAsync(Format.Warning("Could not find a data reference."));
                return;
            }

            await Context.Channel.SendMessageAsync(CatalogViewer.InspectItem(Context, Context.Account, data, slot));
        }

        // This gets a person's backpack.
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inventory"), Alias("backpack", "inv", "bp")]
        [Summary("View your contents currently in storage.")]
        public async Task GetBackpackAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            await Context.Channel.SendMessageAsync(InventoryViewer.Write(account, account.Id == Context.Account.Id));
        }

        [Command("statsof")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetGroupStatsAsync(string query, int page = 1)
        {
            await Context.Channel.SendMessageAsync(StatHelper.WriteFor(Context.Account, query, --page));
        }

        [Command("stats"), Priority(1)]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetStatsAsync(SocketUser user, int page = 1)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            await Context.Channel.SendMessageAsync(StatHelper.Write(account, false, --page));
        }

        [Command("stats"), Priority(0)]
        [RequireUser(AccountHandling.ReadOnly)]
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
        [Command("balance"), Alias("money", "bal", "chips", "tokens", "debt")]
        [Summary("Returns a display showing all of the values in a wallet.")]
        public async Task GetMoneyAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            var provider = new CurrencyFormatter();
            var result = new StringBuilder();

            if (user != null)
            {
                if (user != Context.User)
                    result.AppendLine($"> **Wallet: {account.Username}**");
            }

            if (account.Debt > 0)
                result.AppendLine(string.Format(provider, "{0:D}", account.Debt));
            else if (account.Balance > 0)
                result.AppendLine(string.Format(provider, "{0:O}", account.Balance));

            result.AppendLine(string.Format(provider, "{0:C}", account.ChipBalance));
            result.AppendLine(string.Format(provider, "{0:T}", account.TokenBalance));

            await Context.Channel.SendMessageAsync(result.ToString());
        }

        [RequireUser]
        [Command("clearpalette")]
        [Summary("Remove your currently equipped palette, if any.")]
        public async Task RemovePaletteAsync()
        {
            string name = ItemHelper.NameFor(Context.Account.Card.Palette.Primary, Context.Account.Card.Palette.Secondary);

            if (!ItemHelper.RemovePalette(Context.Account))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You don't have a palette currently equipped."));
                return;
            }

            await Context.Channel.SendMessageAsync($"> Successfully removed **{name}** from your **Card Palette**.");
        }

        //[RequireUser]
        //[Command("cardconfig"), Alias("ccfg")]
        //[Summary("View your current configurations for your **Card**.")]
        public async Task GetCardConfigAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.Card.Display());
        }

        [RequireUser]
        [Command("card")]
        public async Task GetCardAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            try
            {
                using var graphics = new GraphicsService();
                var d = new CardDetails(account, user);
                var p = CardProperties.Default;
                p.Font = account.Card.Font;
                p.Palette = account.Card.Palette.Primary;
                p.PaletteOverride = account.Card.Palette.Build();
                p.Trim = false;
                p.Casing = Casing.Upper;

                System.Drawing.Bitmap card = graphics.DrawCard(d, p);

                await Context.Channel.SendImageAsync(card, $"../tmp/{Context.User.Id}_card.png");
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("profile"), Alias("account", "acc", "pf", "user")]
        [Summary("View a profile.")]
        public async Task ViewProfileAsync(ArcadeUser user = null)
        {
            user ??= Context.Account;

            if (await CatchEmptyAccountAsync(user))
                return;

            await Context.Channel.SendMessageAsync(ProfileViewer.View(user, Context));
        }

        private async Task<bool> CatchEmptyAccountAsync(ArcadeUser reference)
        {
            if (reference == null)
            {
                await Context.Channel.SendMessageAsync("> **Odd.**\n> The user you seek does not exist in this world.");
                return true;
            }

            return false;
        }
    }
}