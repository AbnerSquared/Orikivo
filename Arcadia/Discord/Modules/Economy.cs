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
        [Summary("Send a trade offer to the specified user.")]
        public async Task SendOfferAsync(SocketUser user, [Remainder]string input)
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
        [RequireData]
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
            if (!ItemHelper.CanGift(data.Id, data))
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

            account.Items.Add(data);
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
    }
}
