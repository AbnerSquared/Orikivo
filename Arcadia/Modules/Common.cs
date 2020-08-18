using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Services;
using GraphicsService = Arcadia.Graphics.GraphicsService;
using CardDetails = Arcadia.Graphics.CardDetails;
using CardProperties = Arcadia.Graphics.CardProperties;
using Casing = Arcadia.Graphics.Casing;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Modules
{
    // TODO: Implement shopping
    // - Missions
    // - Shopping
    // - Card Customization

    [Name("Common")]
    [Summary("Generic commands that are commonly used.")]
    public class Common : OriModuleBase<ArcadeContext>
    {
        [RequireUser]
        [Command("boosters")]
        [Summary("View all of your currently equipped boosters.")]
        public async Task ViewBoosterAsync()
        {
            await Context.Channel.SendMessageAsync(BoostHelper.Write(Context.Account));
        }

        [RequireUser]
        [Command("merit")]
        [Summary("View the details of a merit.")]
        public async Task ViewMeritAsync(string meritId)
        {
            if (!MeritHelper.Exists(meritId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find any merits with that ID."));
                return;
            }

            if (!MeritHelper.HasMerit(Context.Account, meritId) && MeritHelper.GetMerit(meritId).Hidden)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You aren't authorized to view this merit."));
                return;
            }

            await Context.Channel.SendMessageAsync(MeritHelper.Write(MeritHelper.GetMerit(meritId), Context.Account));
        }

        [RequireUser]
        [Command("claim")]
        [Summary("Attempt to claim the specified merit.")]
        public async Task ClaimAsync(string meritId)
        {
            if (!MeritHelper.Exists(meritId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find any merits with that ID."));
                return;
            }

            await Context.Channel.SendMessageAsync(MeritHelper.Claim(Context.Account, meritId));
        }

        [RequireUser]
        [Command("shops")]
        public async Task ViewShopsAsync()
        {
            await Context.Channel.SendMessageAsync(ShopHelper.ViewShops());
        }

        [RequireUser]
        [Command("shop")]
        public async Task ShopAsync(string shopId)
        {
            // Ignore initialization of another shop if a shop handle is already open
            if (!Context.Account.CanShop)
                return;

            if (!ShopHelper.Exists(shopId))
            {
                await Context.Channel.WarnAsync("Could not find a shop with the specified ID.");
                return;
            }

            Shop shop = ShopHelper.GetShop(shopId);
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
        public async Task ViewMeritsAsync(MeritQuery flag = MeritQuery.Default)
        {
            await Context.Channel.SendMessageAsync(MeritHelper.View(Context.Account, flag));
        }

        [RequireUser]
        [Command("assign")]
        [Summary("Assign a new set of quests.")]
        public async Task AssignQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.AssignAndDisplay(Context.Account));
        }

        [RequireUser]
        [Command("complete")]
        [Summary("Claim the rewards from all of your completed objectives")]
        public async Task CompleteQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.CompleteAndDisplay(Context.Account));
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
        [Command("objectives"), Alias("missions", "quests", "tasks"), Priority(0)]
        [Summary("View all of your currently assigned quests.")]
        public async Task ViewQuestsAsync()
        {
            await Context.Channel.SendMessageAsync(QuestHelper.View(Context.Account));
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
        [Command("daily")]
        public async Task GetDailyAsync()
        {
            DailyResultFlag result = Daily.Next(Context.Account);
            await Context.Channel.SendMessageAsync(Daily.ApplyAndDisplay(Context.Account, result));
        }

        [RequireUser]
        [Command("giveprogresspioneer")]
        public async Task GivePioneerMerit(SocketUser user = null)
        {
            if (Context.User.Id != OriGlobal.DevId)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Only the developer may execute this command."));
                return;
            }

            user ??= Context.User;
            Context.Data.Users.TryGet(user.Id, out ArcadeUser participant);

            if (participant == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            MeritHelper.TryUnlock(participant, Merits.ProgressPioneer);
            Context.SaveUser(participant);

            if (MeritHelper.HasMerit(participant, Merits.ProgressPioneer))
                await Context.Channel.SendMessageAsync($"> Gave **{participant.Username}** **{MeritHelper.NameOf(Merits.ProgressPioneer)}**.");
        }

        

        //[Command("catalog")]
        //[Command("catalog")]
        [Summary("View your current **Item** catalog.")]
        public async Task GetCatalogAsync()
        {

        }

        [Command("trade")]
        [Summary("Attempts to start a trade with the specified user.")]
        public async Task TradeAsync(SocketUser user)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            var handler = new TradeHandler(Context, account);

            await HandleTradeAsync(handler);
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
        public async Task GiftAsync(SocketUser user, string itemId)
        {
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            // Check if the item exists, similar to use.
            if (!ItemHelper.Exists(itemId))
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```I couldn't find any items with the specified ID.```");
                return;
            }

            if (!ItemHelper.HasItem(Context.Account, itemId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You do not own this item."));
                return;
            }


            // Next, check if the item can be gifted.
            if (!ItemHelper.CanGift(itemId, ItemHelper.DataOf(Context.Account, itemId)?.Data))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("This item cannot be gifted."));
                return;
            }

            // Otherwise, Take the item away from the invoker
            // If the item has a limited gift count, add one to the gift counter and give it to the user.

            ItemHelper.TakeItem(Context.Account, itemId);

            // Give the item to the user.
            ItemHelper.GiveItem(account, itemId);

            await Context.Channel.SendMessageAsync($"> 🎁 Gave **{account.Username}** a **{ItemHelper.NameOf(itemId)}**.");

            if (ItemHelper.GetItem(itemId).GiftLimit.HasValue)
            {
                bool hasGiftCounter = ItemHelper.DataOf(account, itemId)?.Data?.GiftCount.HasValue ?? false;
                if (hasGiftCounter)
                {
                    ItemHelper.DataOf(account, itemId).Data.GiftCount++;
                }
            }
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
                var rem = ItemHelper.GetCooldownRemainder(Context.Account, id);
                if (rem.HasValue)
                {
                    await Context.Channel.SendMessageAsync($"> You can use **{ItemHelper.NameOf(id)}** in {Orikivo.Format.Counter(rem.Value.TotalSeconds)}.");
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

        //[Command("destroy"), Alias("delete", "del")]
        [Summary("Attempts to destroy the specified **Item** by its internal ID.")]
        public async Task DestroyItemAsync(string id)
        {
            var result = ItemHelper.GetItem(id);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```I couldn't find any items with the specified ID.```");
                return;
            }


        }

        [Command("inspect")]
        [Summary("Provides details about the specified **Item**, if it has been previously discovered.")]
        public async Task InspectAsync(string id)
        {
            var result = ItemHelper.GetItem(id);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```I couldn't find any items with the specified ID.```");
                return;
            }

            //Console.WriteLine(ItemHelper.GetUniqueId());

            await Context.Channel.SendMessageAsync(Catalog.WriteItem(result));
        }

        // This gets a person's backpack.
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("inventory"), Alias("backpack", "items", "inv", "bp")]
        [Summary("View your contents currently in storage.")]
        public async Task GetBackpackAsync(SocketUser user = null) // int page = 0
        {
            user ??= Context.User;
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            await Context.Channel.SendMessageAsync(Inventory.Write(account, account.Id == Context.Account.Id));
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