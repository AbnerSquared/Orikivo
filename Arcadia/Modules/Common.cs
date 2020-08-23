using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Services;
using Arcadia.Graphics;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    // - Card Customization

    [Name("Common")]
    [Summary("Generic commands that are commonly used.")]
    public class Common : OriModuleBase<ArcadeContext>
    {
        [Command("guides")]
        [Summary("Learn how to use **Orikivo Arcade** with this collection of guides.")]
        public async Task ViewGuidesAsync(int page = 1)
        {
            page--;
            await Context.Channel.SendMessageAsync(GuideHelper.View(page));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("recipes")]
        [Summary("View all of your currently known recipes.")]
        public async Task ViewRecipesAsync()
        {
            await Context.Channel.SendMessageAsync(Catalog.ViewRecipes(Context.Account));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("recipe")]
        [Summary("View information about a specific recipe.")]
        public async Task ViewRecipeAsync([Name("recipe_id")] Recipe recipe)
        {
            /*
            if (!ItemHelper.RecipeExists(recipeId))
            {
                await Context.Channel.WarnAsync("Could not find the specified recipe.");
                return;
            }*/

            await Context.Channel.SendMessageAsync(Catalog.ViewRecipeInfo(Context.Account, recipe));
        }

        [RequireUser]
        [Command("craft")]
        [Summary("Craft an item from the specified recipe.")]
        public async Task CraftAsync([Name("recipe_id")]Recipe recipe)
        {
            /*
            if (!ItemHelper.RecipeExists(recipeId))
            {
                await Context.Channel.WarnAsync("Could not find the specified recipe.");
                return;
            }*/

            if (!ItemHelper.CanCraft(Context.Account, recipe))
            {
                var notice = new StringBuilder();

                notice.AppendLine(Format.Warning("You are unable to craft this recipe."));
                notice.AppendLine("\n> **Missing Components**");

                foreach ((string itemId, int amount) in ItemHelper.GetMissingFromRecipe(Context.Account, recipe))
                {
                    notice.AppendLine(Catalog.WriteRecipeComponent(itemId, amount));
                }

                await Context.Channel.SendMessageAsync(notice.ToString());
                return;
            }


            if (ItemHelper.Craft(Context.Account, recipe))
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
        [Command("boosters")]
        [Summary("View all of your currently equipped boosters.")]
        public async Task ViewBoostersAsync()
        {
            await Context.Channel.SendMessageAsync(BoostHelper.Write(Context.Account));
        }

        [RequireUser]
        [Command("merit")]
        [Summary("View the details of a merit.")]
        public async Task ViewMeritAsync(Merit merit)
        {
            /*
            if (!MeritHelper.Exists(meritId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find any merits with that ID."));
                return;
            }*/

            if (!MeritHelper.HasMerit(Context.Account, merit) && merit.Hidden)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You aren't authorized to view this merit."));
                return;
            }

            await Context.Channel.SendMessageAsync(MeritHelper.Write(merit, Context.Account));
        }

        [RequireUser]
        [Command("cooldowns"), Alias("cooldown", "expiry")]
        public async Task ViewCooldownsAsync()
        {
            await Context.Channel.SendMessageAsync(CooldownHelper.ViewAllTimers(Context.Account));
        }

        [RequireUser]
        [Command("claim")]
        [Summary("Attempt to claim the specified merit.")]
        public async Task ClaimAsync(Merit merit)
        {
            /*
            if (!MeritHelper.Exists(meritId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find any merits with that ID."));
                return;
            }*/

            await Context.Channel.SendMessageAsync(MeritHelper.Claim(Context.Account, merit));
        }

        [RequireUser]
        [Command("shops")]
        public async Task ViewShopsAsync()
        {
            await Context.Channel.SendMessageAsync(ShopHelper.ViewShops(Context.Data.Data));
        }

        [RequireUser]
        [Command("stat")]
        public async Task ViewStatAsync(string id)
        {
            await Context.Channel.SendMessageAsync(Var.ViewDetails(Context.Account, id));
        }

        [RequireUser]
        [RequireData]
        [Command("shop")]
        public async Task ShopAsync(Shop shop)
        {
            // Ignore initialization of another shop if a shop handle is already open
            if (!Context.Account.CanShop)
                return;
            /*
            if (!ShopHelper.Exists(shopId))
            {
                await Context.Channel.WarnAsync("Could not find a shop with the specified ID.");
                return;
            }
            */

            //Shop shop = ShopHelper.GetShop(shopId);
            var handle = new ShopHandler(Context, shop);

            await HandleShopAsync(handle);
            Context.Account.CanShop = true;
        }

        private async Task HandleShopAsync(ShopHandler shop)
        {
            try
            {
                var collector = new MessageCollector(Context.Client);

                var options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(30),
                    Action = shop
                };

                bool Filter(SocketMessage message, int index)
                {
                    return (message.Author.Id == Context.User.Id) && (message.Channel.Id == Context.Channel.Id);
                }

                await collector.MatchAsync(Filter, options);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        [RequireUser]
        [Command("claimall")]
        [Summary("Attempt to claim all available merits.")]
        public async Task ClaimAllAsync()
        {
            await Context.Channel.SendMessageAsync(MeritHelper.ClaimAll(Context.Account));
        }

        [RequireUser]
        [Command("merits")]
        [Summary("View the directory of accomplishments.")]
        public async Task ViewMeritsAsync(MeritQuery flag = MeritQuery.Default, int page = 1)
        {
            page--;
            await Context.Channel.SendMessageAsync(MeritHelper.View(Context.Account, flag, page));
        }

        // NOTE: QUESTS

        [RequireUser]
        [Command("assign")]
        [Summary("Assign a new set of quests.")]
        public async Task AssignQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.AssignAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("objectives"), Alias("missions", "quests", "tasks"), Priority(1)]
        [Summary("View the currently assigned objective on the specified slot.")]
        public async Task ViewQuestAsync(int slot)
        {
            slot--;
            await Context.Channel.SendMessageAsync(QuestHelper.ViewSlot(Context.Account, slot));
        }

        [RequireUser]
        [Command("objectives"), Alias("missions", "quests", "tasks"), Priority(0)]
        [Summary("View all of your currently assigned quests.")]
        public async Task ViewQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.View(Context.Account));
        }

        [RequireUser]
        [Command("toss")]
        [Summary("Toss the specified objective you are currently working on.")]
        public async Task TossQuestAsync(int slot)
        {
            slot--;
            await Context.Channel.SendMessageAsync(QuestHelper.TossSlot(Context.Account, slot));
        }

        [RequireUser]
        [Command("complete")]
        [Summary("Claim the rewards from all of your completed objectives")]
        public async Task CompleteQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.CompleteAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("daily")]
        public async Task GetDailyAsync()
        {
            DailyResultFlag result = Daily.Next(Context.Account);
            await Context.Channel.SendMessageAsync(Daily.ApplyAndDisplay(Context.Account, result));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("catalog")]
        [Summary("View all of the items you have seen or known about.")]
        public async Task ViewCatalogAsync(string query = null, int page = 1)
        {
            page--;
            await Context.Channel.SendMessageAsync(Catalog.View(Context.Account, query, page));
        }

        [Command("trade")]
        [Summary("Attempts to start a trade with the specified user.")]
        public async Task TradeAsync(SocketUser user)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            if (!Context.Account.CanTrade)
                return;

            Context.Account.CanTrade = false;

            var handler = new TradeHandler(Context, account);

            await HandleTradeAsync(handler);
            Context.Account.CanTrade = true;
            account.CanTrade = true;
        }

        private async Task HandleTradeAsync(TradeHandler handler)
        {
            try
            {
                var collector = new MessageCollector(Context.Client);
                var options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(20),
                    Action = handler
                };

                bool Filter(SocketMessage message, int index)
                {
                    return (handler.Host.Id == message.Author.Id || handler.Participant.Id == message.Author.Id)
                        && (message.Channel.Id == Context.Channel.Id);
                }

                await collector.MatchAsync(Filter, options);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        [Command("gift")]
        [Summary("Attempts to gift an **Item** to the specified user.")]
        public async Task GiftAsync(SocketUser user, Item item)
        {
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            /*
            // Check if the item exists, similar to use.
            if (!ItemHelper.Exists(itemId))
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```I couldn't find any items with the specified ID.```");
                return;
            }*/

            if (!ItemHelper.HasItem(Context.Account, item))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You do not own this item."));
                return;
            }

            // Next, check if the item can be gifted.
            if (!ItemHelper.CanGift(item, ItemHelper.DataOf(Context.Account, item)?.Data))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("This item cannot be gifted."));
                return;
            }

            // Otherwise, Take the item away from the invoker
            // If the item has a limited gift count, add one to the gift counter and give it to the user.

            ItemHelper.TakeItem(Context.Account, item);

            // Give the item to the user.
            ItemHelper.GiveItem(account, item);

            await Context.Channel.SendMessageAsync($"> 🎁 Gave **{account.Username}** a **{item.GetName()}**.");

            if (item.GiftLimit.HasValue)
            {
                bool hasGiftCounter = ItemHelper.DataOf(account, item)?.Data?.GiftCount.HasValue ?? false;

                if (hasGiftCounter)
                    ItemHelper.DataOf(account, item).Data.GiftCount++;
            }

            Context.Account.AddToVar(Stats.ItemsGifted);
        }

        [Command("use")]
        [Summary("Uses the specified **Item** by its internal or unique ID.")]
        public async Task UseItemAsync(string id)
        {
            // TODO: Handle the using of unique items.

            if (!ItemHelper.Exists(id))
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```I couldn't find any items with the specified ID.```");
                return;
            }

            if (!ItemHelper.HasItem(Context.Account, id))
            {
                await Context.Channel.SendMessageAsync("> You do not own this item.");
                return;
            }

            if (!ItemHelper.CanUse(Context.Account, id))
            {
                TimeSpan? rem = ItemHelper.GetCooldownRemainder(Context.Account, id);
                if (rem.HasValue)
                {
                    await Context.Channel.SendMessageAsync($"> You can use **{ItemHelper.NameOf(id)}** in {Format.Counter(rem.Value.TotalSeconds)}.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Format.Warning($"You are unable to use **{ItemHelper.NameOf(id)}**."));
                }

                return;
            }

            UsageResult result = ItemHelper.UseItem(Context.Account, id);

            if (result.Message != null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, result.Message);
                return;
            }

            await Context.Channel.SendMessageAsync(Format.Warning($"You have used **{ItemHelper.NameOf(id)}**."));
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inspect")]
        [Summary("Provides details about the specified **Item**, if it has been previously discovered.")]
        public async Task InspectAsync(Item item)
        {
            await Context.Channel.SendMessageAsync(Catalog.ViewItem(item, ItemHelper.GetCatalogStatus(Context.Account, item)));
        }

        // This gets a person's backpack.
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inventory"), Alias("backpack", "items", "inv", "bp")]
        [Summary("View your contents currently in storage.")]
        public async Task GetBackpackAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            await Context.Channel.SendMessageAsync(Inventory.Write(account, account.Id == Context.Account.Id));
        }

        [Command("statsof"), Priority(1)]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetGroupStatsAsync(string query, int page = 0)
        {
            await Context.Channel.SendMessageAsync(StatHelper.WriteFor(Context.Account, query, page));
        }

        [Command("stats"), Priority(1)]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetStatsAsync(SocketUser user, int page = 0)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            await Context.Channel.SendMessageAsync(StatHelper.Write(account, false, page));
        }

        [Command("stats"), Priority(1)]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetStatsAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(StatHelper.Write(Context.Account, page: --page));
        }

        [Command("leaderboard"), Alias("top"), Priority(0)]
        [Summary("Filters a custom leaderboard based on a specified **Stat**.")]
        public async Task GetLeaderboardAsync(string statId, LeaderboardSort sort = LeaderboardSort.Most, int page = 0)
        {
            var board = new Leaderboard(statId, sort);

            string result = board.Write(Context.Data.Users.Values.Values, page);

            await Context.Channel.SendMessageAsync(result);
        }

        [Command("localboard"), Alias("toplocal"), Priority(1)]
        [Summary("View the local pioneers of a specific category.")]
        public async Task GetLocalboardAsync(LeaderboardQuery flag = LeaderboardQuery.Default,
            LeaderboardSort sort = LeaderboardSort.Most, int page = 0)
        {
            if (flag == LeaderboardQuery.Custom)
                flag = LeaderboardQuery.Default;

            var board = new Leaderboard(flag, sort);
            // Keep note that this may return null by mistake
            string result = board.Write(Context.Data.Users.Values.Values.Where(x => Context.Guild.GetUser(x.Id) != null), page);

            await Context.Channel.SendMessageAsync(result);
        }

        // TODO: Implement enum value listings
        [Command("leaderboard"), Alias("top"), Priority(1)]
        [Summary("View the current pioneers of a specific category.")]
        public async Task GetLeaderboardAsync(LeaderboardQuery flag = LeaderboardQuery.Default, LeaderboardSort sort = LeaderboardSort.Most, int page = 0)
        {
            if (flag == LeaderboardQuery.Custom)
                flag = LeaderboardQuery.Default;

            var board = new Leaderboard(flag, sort);
            string result = board.Write(Context.Data.Users.Values.Values, page);

            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("balance"), Alias("money", "bal")]
        [Summary("Returns a current wallet state.")]
        public async Task GetMoneyAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            var values = new StringBuilder();
            if (user != null)
            {
                if (user != Context.User)
                    values.AppendLine($"**Wallet - {account.Username}**");
            }

            values.AppendLine($"**Balance**: 💸 **{account.Balance:##,0}**");
            values.AppendLine($"**Chips**: 🧩 **{account.ChipBalance:##,0}**");
            values.AppendLine($"**Tokens**: 🏷️ **{account.TokenBalance:##,0}**");
            values.AppendLine($"**Debt**: 📃 **{account.Debt:##,0}**");

            await Context.Channel.SendMessageAsync(values.ToString());
        }

        [RequireUser]
        [Command("clearpalette")]
        [Summary("Remove your currently equipped palette, if any.")]
        public async Task RemovePaletteAsync()
        {
            string name = ItemHelper.NameFor(Context.Account.Card.Palette.Primary, Context.Account.Card.Palette.Secondary);

            if (!ItemHelper.RemovePalette(Context.Account))
            {
                await Context.Channel.WarnAsync("You don't have a palette currently equipped.");
                return;
            }

            await Context.Channel.SendMessageAsync($"> Successfully removed **{name}** from your **Card Palette**.");
        }

        [RequireUser]
        [Command("cardconfig"), Alias("ccfg")]
        [Summary("View your current configurations for your **Card**.")]
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

        private async Task<bool> CatchEmptyAccountAsync(ArcadeUser reference)
        {
            if (reference == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return true;
            }

            return false;
        }
    }
}