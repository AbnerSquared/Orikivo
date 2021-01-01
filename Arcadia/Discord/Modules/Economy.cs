using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arcadia.Multiplayer;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Text;

namespace Arcadia.Modules
{
    [Icon("💴")]
    [Name("Economy")]
    [Summary("Commands related to economical features.")]
    public class Economy : ArcadeModule
    {
        private readonly LocaleProvider _locale;
        public Economy(LocaleProvider locale)
        {
            _locale = locale;
        }

        // offer send <user> <outbound> for <inbound>
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
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_empty_offer", Context.Account.Config.Language)));
                return;
            }

            if (!TradeHelper.TryParseOffer(Context.User, user, input, out TradeOffer offer))
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_offer_parse_failed", Context.Account.Config.Language)));
                return;
            }

            string result = TradeHelper.SendOffer(Context.Account, account, offer);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        // offer accept <id>
        [RequireUser]
        [Command("offeraccept")]
        [Summary("Accepts the specified trade offer.")]
        public async Task AcceptOfferAsync([Name("offer_id")]string offerId)
        {
            if (Context.Account.Offers.All(x => x.Id != offerId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_unknown_offer", Context.Account.Config.Language)));
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

        // offer decline <id>
        [RequireUser]
        [Command("offerdecline"), Alias("offercancel")]
        [Summary("Declines or cancels the specified trade offer.")]
        public async Task DeclineOfferAsync([Name("offer_id")]string offerId)
        {
            if (Context.Account.Offers.All(x => x.Id != offerId))
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_unknown_offer", Context.Account.Config.Language)));
                return;
            }

            TradeOffer offer = Context.Account.Offers.First(x => x.Id == offerId);

            if (!offer.Author.Id.HasValue)
                throw new Exception("Expected author ID to be specified in trade offer");

            if (!Context.TryGetUser(offer.Author.Id.Value, out ArcadeUser account))
                throw new Exception("Expected user account to exist from the specified offer");

            string result = TradeHelper.DeclineOffer(Context.Account, account, offer);
            await Context.Channel.SendMessageAsync(Context.Account, result);
        }

        // offers <page>
        [Command("offers")]
        [Summary("View all of your active trade offers.")]
        public async Task ViewOffersAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(TradeHelper.ViewOffers(Context.Account, Context, --page));
        }

        [RequireUser]
        [Command("shops")]
        [Summary("View your collection of available shops to visit.")]
        public async Task ViewShopsAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(ShopHelper.ViewShops(Context.Account, --page));
        }

        [Session]
        [RequireUser]
        [RequireGlobalData]
        [Command("shop")]
        [Summary("Starts a session for the specified **Shop**.")]
        public async Task ShopAsync(Shop shop = null)
        {
            if (shop == null)
            {
                await Context.Channel.SendMessageAsync($"{Format.Warning(_locale.GetValue("warning_shop_unspecified", Context.Account.Config.Language))}\n\n{ShopHelper.ViewShops(Context.Account)}");
                return;
            }

            var session = new ShopSession(Context, shop);
            await StartSessionAsync(session);
        }

        // [Command("trade")]
        [Summary("Attempts to start a trade session with the specified user.")]
        public async Task TradeAsync(SocketUser user)
        {
            Context.TryGetUser(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            if (Context.Account.IsInSession)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_is_in_session", Context.Account.Config.Language)));
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
        public async Task GiftAsync([Summary("The user to send your gift to.")]SocketUser user, [Name("data_id")][Summary("The specified item data instance to gift.")]string dataId)
        {
            Context.Data.Users.TryGet(user.Id, out ArcadeUser account);

            if (await CatchEmptyAccountAsync(account))
                return;

            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);

            if (data == null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_invalid_data_reference", Context.Account.Config.Language)));
                return;
            }

            if (account.Id == Context.Account.Id)
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_gift_target_self", Context.Account.Config.Language)));
                return;
            }

            Item item = ItemHelper.GetItem(data.Id);

            if (!ItemHelper.CanGift(account, data.Id, data))
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_invalid_gift", Context.Account.Config.Language)));
                return;
            }

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
            Context.Account.AddToVar(Stats.Common.ItemsGifted);

            await Context.Channel.SendMessageAsync(Context.Account, $"> 🎁 Gave **{account.Username}** {(data.Seal != null ? "an item" : $"**{ItemHelper.NameOf(data.Id)}**")}.");
        }

        [RequireUser]
        [Command("orders")]
        [Summary("View all of your currently active orders.")]
        public async Task ViewOrdersAsync(int page = 1)
        {
            await Context.Channel.SendMessageAsync(CatalogHelper.ViewOrders(Context.Account, --page));
        }

        [RequireUser]
        [Command("order")]
        [Summary("Orders the specified **Item** from your catalog (at a 125% markup value).")]
        public async Task OrderItemAsync(Item item)
        {
            long orderLimit = Var.GetOrSet(Context.Account, Vars.OrderLimit, 3);

            if (CatalogHelper.GetOrderCount(Context.Account) >= orderLimit)
            {
                await Context.Channel.SendMessageAsync(Format.Warning(_locale.GetValue("warning_order_overflow", Context.Account.Config.Language)));
                return;
            }

            CatalogStatus status = CatalogHelper.GetCatalogStatus(Context.Account, item);

            if (!(item.Tags.HasFlag(ItemTag.Orderable) && status >= CatalogStatus.Known))
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_order_not_allowed", Context.Account.Config.Language)));
                return;
            }

            long cost = BoostConvert.GetValue(item.Value, 1.25f);

            if (!CatalogHelper.CanAfford(Context.Account, cost, item.Currency))
            {
                await Context.Channel.SendMessageAsync(Context.Account,
                    Format.Warning(_locale.GetValue("warning_order_not_afford", Context.Account.Config.Language)));
                return;
            }

            CatalogHelper.TryAddOrder(Context.Account, item);
            Context.Account.Take(cost, item.Currency);
            ItemHelper.GiveItem(Context.Account, item);

            await Context.Channel.SendMessageAsync(Context.Account,
                _locale.GetValue("order_success", Context.Account.Config.Language, Format.Bold(item.Name), CurrencyHelper.WriteCost(cost, item.Currency)));
                // $"> You have placed an order for **{item.Name}** ({CurrencyHelper.WriteCost(cost, item.Currency)}).");
        }

        [RequireUser]
        [Command("sell")]
        [Summary("Sells the specified **Item** to the desired **Shop**.")]
        public async Task SellItemAsync([Name("data_id")][Summary("The specified item data instance to sell.")]string dataId, [Summary("The **Shop** to sell your **Item** to.")]Shop shop)
        {
            ItemData data = ItemHelper.GetItemData(Context.Account, dataId);

            if (data == null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_invalid_data_reference", Context.Account.Config.Language)));
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
            IEnumerable<ItemData> disposable = Context.Account.Items.Where(ItemHelper.CanDelete);

            ItemData target = disposable.FirstOrDefault(x =>
                (ItemHelper.Exists(x.Id) && (x.Id == dataId || x.Data?.Id == dataId)) || x.TempId == dataId);

            if (target == null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, Format.Warning(_locale.GetValue("warning_invalid_delete_target", Context.Account.Config.Language)));
                return;
            }

            ItemHelper.DeleteItem(Context.Account, target);
            string targetName = ItemHelper.GetItem(target.Id)?.GetName() ?? ItemHelper.GetItem(Ids.Items.InternalUnknown).Name;
            await Context.Channel.SendMessageAsync(Context.Account, $"> {_locale.GetValue("delete_success", Context.Account.Config.Language, Format.Bold(targetName))}");
        }

        private async Task<bool> CatchEmptyAccountAsync(ArcadeUser reference)
        {
            if (reference == null)
            {
                await Context.Channel.SendMessageAsync(Context.Account, $"> **Odd.**\n> {_locale.GetValue("warning_account_not_found", Context.Account.Config.Language)}");
                return true;
            }

            return false;
        }
    }
}
