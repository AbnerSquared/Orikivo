using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Services;
using Arcadia.Graphics;
using GraphicsService = Arcadia.Graphics.GraphicsService;
using CardDetails = Arcadia.Graphics.CardDetails;
using CardProperties = Arcadia.Graphics.CardProperties;
using Casing = Arcadia.Graphics.Casing;

namespace Arcadia.Modules
{
    // TODO: Instead of being an enum value, simply make the flag NULL

    // TODO: Implement shopping, merits
    // - Missions
    // - Shopping
    // - Card Customization
    // - Merits
    [Name("Common")]
    [Summary("Generic commands that are commonly used.")]
    public class Common : OriModuleBase<ArcadeContext>
    {
        [RequireUser]
        [Command("daily")]
        public async Task GetDailyAsync()
        {
            DailyResultFlag result = Daily.Next(Context.Account);
            await Context.Channel.SendMessageAsync(Daily.ApplyAndDisplay(Context.Account, result));
        }

        //[Command("testgiveitem")]
        [RequireUser]
        public async Task GiveItemAsync(SocketUser user = null)
        {
            try
            {
                user ??= Context.User;
                Context.Data.Users.TryGet(user.Id, out ArcadeUser participant);

                if (participant == null)
                {
                    await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                    return;
                }

                int owned = ItemHelper.GetOwnedAmount(participant, Items.PaletteGlass);

                ItemHelper.GiveItem(participant, Items.PaletteGlass);

                if (ItemHelper.GetOwnedAmount(participant, Items.PaletteGlass) > owned)
                {
                    await Context.Channel.SendMessageAsync($"> Gave **{participant.Username}** a **{ItemHelper.NameOf(Items.PaletteGlass)}**.");
                }
                else if (ItemHelper.GetOwnedAmount(participant, Items.PaletteGlass) == owned)
                {
                    await Context.Channel.SendMessageAsync($"> Could not give **{participant.Username}** a **{ItemHelper.NameOf(Items.PaletteGlass)}**. They already own the most possible.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"> I don't know what happened for this message to appear.");
                }
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        
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
            Context.Data.Users.TryGet(user.Id, out ArcadeUser participant);

            if (participant == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            var handler = new TradeHandler(Context, participant);

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

                Func<SocketMessage, int, bool> filter = delegate (SocketMessage message, int index)
                {
                    return (handler.Host.Id == message.Author.Id || handler.Participant.Id == message.Author.Id)
                        && (message.Channel.Id == Context.Channel.Id);
                };

                await collector.MatchAsync(filter, options);
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
                await Context.Channel.SendMessageAsync("> You do not own this item.");
                return;
            }


            // Next, check if the item can be gifted.
            if (!ItemHelper.CanGift(itemId, ItemHelper.DataOf(Context.Account, itemId)?.Data))
            {
                await Context.Channel.SendMessageAsync("> This item cannot be gifted.");
                return;
            }

            // Otherwise, Take the item away from the invoker
            // If the item has a limited gift count, add one to the gift counter and give it to the user.

            ItemHelper.TakeItem(Context.Account, itemId);

            // Give the item to the user.
            ItemHelper.GiveItem(account, itemId);

            await Context.Channel.SendMessageAsync($"> Gave **{account.Username}** a **{ItemHelper.NameOf(itemId)}**.");

            /*
            if (ItemHelper.GetItem(itemId).GiftLimit.HasValue)
            {
                bool hasGiftCounter = ItemHelper.DataOf(account, itemId)?.Data?.GiftCount.HasValue ?? false;
                //if (hasGiftCounter)
                //   ItemHelper.DataOf(account, itemId)?.Data?.GiftCount += 1;
            }*/

        }

        [Command("use")]
        [Summary("Uses the specified **Item** by its internal or unique ID.")]
        public async Task UseItemAsync(string id)
        {
            // TODO: Handle the using of unique items.
            /*
            // if this ID was a valid unique id
            string itemId = ItemHelper.ItemOf(Context.Account, id);

            if (!string.IsNullOrWhiteSpace(itemId))
            {
                ItemHelper.UseItem(Context.Account, itemId, id);
                await Context.Channel.SendMessageAsync($"> You have used **{ItemHelper.NameOf(itemId)}**.");
                return;
            }
            */

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
                    await Context.Channel.SendMessageAsync($"> You are unable to use **{ItemHelper.NameOf(id)}**.");
                }

                return;
            }

            ItemHelper.UseItem(Context.Account, id);
            await Context.Channel.SendMessageAsync($"> You have used **{ItemHelper.NameOf(id)}**.");
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
        [Command("inventory"), Alias("backpack", "items", "bp")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetBackpackAsync(int page = 0, SocketUser user = null)
        {
            user ??= Context.User;
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            await Context.Channel.SendMessageAsync(Inventory.Write(account));
        }

        [Command("stats")]
        [RequireUser(AccountHandling.ReadOnly)]
        public async Task GetStatsAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            string stats = string.Join("\n",
                account.Stats.Where((key, value) => !key.StartsWith("cooldown")
                && value != 0
                && !ItemHelper.Items.Select(x => ItemHelper.GetCooldownId(x.Id)).Contains(key)).Select(s => $"`{s.Key}`: {s.Value}"));

            if (string.IsNullOrWhiteSpace(stats))
            {
                stats = "*No stats have been specified!*";
            }

            if (Context.User.Id != user.Id)
                stats = $"> **Stats - {user.Username}**\n\n" + stats;

            await Context.Channel.SendMessageAsync(stats);
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
        public async Task GetLeaderboardAsync(LeaderboardFlag flag = LeaderboardFlag.Default, LeaderboardSort sort = LeaderboardSort.Most, int page = 0)
        {
            if (flag == LeaderboardFlag.Custom)
                flag = LeaderboardFlag.Default;

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
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

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
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (account == null)
            {
                await Context.Channel.SendMessageAsync("> **Oops!**\n> I ran into an issue.\n```The specified user does not seem to have an account.```");
                return;
            }

            try
            {
                using var graphics = new GraphicsService();
                var d = new CardDetails(account, user);
                var p = CardProperties.Default;

                p.Palette = account.Card.Palette;
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
    }
}