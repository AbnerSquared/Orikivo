using System;
using System.Linq;
using System.Threading.Tasks;
using Arcadia.Multiplayer;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo;

namespace Arcadia.Modules
{
    [Icon("💴")]
    [Name("Economy")]
    [Summary("Commands related to economical features.")]
    public class Economy : BaseModule<ArcadeContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfigurationRoot _config;

        public Economy(DiscordSocketClient client, IConfigurationRoot config)
        {
            _client = client;
            _config = config;
        }

        [RequireUser]
        [Command("offer")]
        [Summary("Sends a trade offer to the specified user.")]
        public async Task SendOfferAsync(SocketUser user,
            [Remainder][Summary("The input representing the trade offer to create.")][Tooltip("Type `guide beginner 4` to learn more about how to create trade offers.")] string input)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            if (!Check.NotNull(input))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You must specify your offer contents."));
                return;
            }

            if (!TradeHelper.TryParseOffer(Context.User, user, input, out TradeOffer offer))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("An error has occurred while trying to parse the specified trade offer."));
                return;
            }

            string result = TradeHelper.SendOffer(Context.Account, account, offer);
            await Context.Channel.SendMessageAsync(result);
        }

        // Instead of using the term 'author' for trade offers, try using 'creator' or 'composer' instead
        [RequireUser]
        [Command("offeraccept")]
        [Summary("Accepts the specified trade offer.")]
        public async Task AcceptOfferAsync([Name("offer_id")]string offerId)
        {
            if (Context.Account.Offers.All(x => x.Id != offerId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find an offer with the specified ID."));
                return;
            }

            TradeOffer offer = Context.Account.Offers.First(x => x.Id == offerId);

            if (!offer.Author.Id.HasValue)
                throw new Exception("Expected author ID to be specified in trade offer");

            if (!Context.TryGetUser(offer.Author.Id.Value, out ArcadeUser account))
                throw new Exception("Expected user account to exist from the specified offer");

            string result = TradeHelper.AcceptOffer(Context.Account, account, offer);
            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUser]
        [Command("offerdecline"), Alias("offercancel")]
        [Summary("Declines or cancels the specified trade offer.")]
        public async Task DeclineOfferAsync([Name("offer_id")]string offerId)
        {
            if (Context.Account.Offers.All(x => x.Id != offerId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find an offer with the specified ID."));
                return;
            }

            TradeOffer offer = Context.Account.Offers.First(x => x.Id == offerId);

            if (!offer.Author.Id.HasValue)
                throw new Exception("Expected author ID to be specified in trade offer");

            if (!Context.TryGetUser(offer.Author.Id.Value, out ArcadeUser account))
                throw new Exception("Expected user account to exist from the specified offer");

            string result = TradeHelper.DeclineOffer(Context.Account, account, offer);
            await Context.Channel.SendMessageAsync(result);
        }

        [Command("offers")]
        [Summary("View all of the possible trade offers requested to you.")]
        public async Task ViewOffersAsync()
        {
            await Context.Channel.SendMessageAsync(TradeHelper.ViewOffers(Context.Account, Context));
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

        [RequireUser]
        [Command("shops")]
        [Summary("View your collection of available shops to visit.")]
        public async Task ViewShopsAsync()
        {
            await Context.Channel.SendMessageAsync(ShopHelper.ViewShops(Context.Account));
        }

        [Session]
        [RequireUser]
        [RequireGlobalData]
        [Command("shop")]
        [Summary("Enter the specified **Shop**.")]
        public async Task ShopAsync(Shop shop)
        {
            var session = new ShopSession(Context, shop);
            await StartSessionAsync(session);
        }

        [Command("trade")]
        [Summary("Attempts to start a trade with the specified user.")]
        public async Task TradeAsync(SocketUser user)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            if (Context.Account.IsInSession)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You are already in another active session."));
                return;
            }

            Context.Account.IsInSession = true;

            var session = new TradeSession(Context, account);
            await StartSessionAsync(session, TimeSpan.FromSeconds(20));
            Context.Account.IsInSession = false;
            account.IsInSession = true;
        }


        [Command("gift")]
        [Summary("Attempts to gift an **Item** to the specified user.")]
        public async Task GiftAsync(SocketUser user, [Name("data_id")][Summary("The specified item data instance to gift.")]string dataId)
        {
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);

            if (data == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find a data reference."));
                return;
            }

            if (account.Id == Context.Account.Id)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("You can't send a gift to yourself."));
                return;
            }

            Item item = ItemHelper.GetItem(data.Id);

            // Next, check if the item can be gifted.
            if (!ItemHelper.CanGift(account, data.Id, data))
            {
                await Context.Channel.SendMessageAsync(Format.Warning("This item cannot be gifted."));
                return;
            }

            // Otherwise, Take the item away from the invoker
            // If the item has a limited gift count, add one to the gift counter and give it to the user.

            Context.Account.Items.Remove(data);

            if (data.Seal != null)
                data.Seal.SenderId = Context.Account.Id;

            if (item.TradeLimit.HasValue)
            {
                bool hasGiftCounter = data.Data?.TradeCount.HasValue ?? false;

                if (hasGiftCounter)
                    ItemHelper.DataOf(account, item).Data.TradeCount++;
            }


            if (account.Config.CanNotify(NotifyAllow.GiftInbound))
            {
                account.Notifier.Add($"You have received a gift! Type `inspect {data.TempId}` to learn more.");
            }

            account.Items.Add(data);
            Context.Data.Users.Save(account); // Save internally since it's not automated
            await Context.Channel.SendMessageAsync($"> 🎁 Gave **{account.Username}** {(data.Seal != null ? "an item" : $"**{ItemHelper.NameOf(data.Id)}**")}.");
            Context.Account.AddToVar(Stats.Common.ItemsGifted);
        }

        [RequireUser]
        [Command("sell")]
        [Summary("Sells the specified **Item** to the desired **Shop**.")]
        public async Task SellItemAsync([Name("data_id")][Summary("The specified item data instance to sell.")]string dataId, [Summary("The **Shop** to sell your **Item** to.")]Shop shop)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);

            if (data == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("Could not find a data reference."));
                return;
            }

            string result = ShopHelper.Sell(shop, data, Context.Account);

            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        [RequireUser]
        [Command("delete"), Alias("del")]
        [Summary("Deletes the specified **Item** from your inventory.")]
        public async Task DeleteItemAsync([Name("data_id")][Summary("The specified item data instance to delete.")]string dataId)
        {
            var deletable = Context.Account.Items.Where(ItemHelper.CanDelete);

            ItemData target = deletable.FirstOrDefault(x =>
                (ItemHelper.Exists(x.Id) && (x.Id == dataId || x.Data?.Id == dataId)) || x.TempId == dataId);

            if (target == null)
            {
                await Context.Channel.SendMessageAsync(Format.Warning("An unknown or indestructable data instance was specified."));
                return;
            }

            ItemHelper.DeleteItem(Context.Account, target);
            // TODO: Set the default names for items to always point at Ids.Items.InternalUnknown
            await Context.Channel.SendMessageAsync($"> Deleted **{ItemHelper.GetItem(target.Id)?.GetName() ?? ItemHelper.GetItem(Ids.Items.InternalUnknown).Name}** from your inventory pool.");
        }
    }
}
