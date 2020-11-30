using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Collectors;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Text;
using Orikivo.Text.Pagination;
using Format = Orikivo.Format;

namespace Arcadia
{
    public class ShopSession : MessageSession
    {
        public ShopSession(ArcadeContext context, Shop shop)
        {
            Context = context;
            Shop = shop;
            State = ShopState.Enter;
            Catalog = context.Data.Data.GetOrGenerateCatalog(shop, context.Account);
            List<Vendor> possibleVendors = ShopHelper.GetVendors(shop.AllowedSellGroups).ToList();
            Vendor = Check.NotNullOrEmpty(possibleVendors) ? Randomizer.Choose(possibleVendors) : null;

            if (!context.Account.CatalogHistory.ContainsKey(shop.Id))
                context.Account.CatalogHistory[shop.Id] = new CatalogHistory();

            if (!context.Account.CatalogHistory[shop.Id].HasVisited)
            {
                context.Account.CatalogHistory[shop.Id].HasVisited = true;
                context.Account.AddToVar(ShopHelper.GetVisitId(shop.Id));
            }

            Var.SetIfEmpty(context.Account, ShopHelper.GetTierId(shop.Id), 1);

            long tier = context.Account.GetVar(ShopHelper.GetTierId(shop.Id));

            if (Check.NotNullOrEmpty(shop.CriteriaTiers)
                && shop.CriteriaTiers.ContainsKey(tier + 1)
                && shop.CriteriaTiers[tier + 1].All(x => Var.MeetsCriterion(context.Account, x)))
                context.Account.AddToVar(ShopHelper.GetTierId(shop.Id));

            PageIndex = 0;
            PageCount = Paginate.GetPageCount(Catalog.ItemIds.Count, ShopHelper.GroupSize);
            KnownShopCount = ShopHelper.GetKnownShops(context.Account).Count();
        }

        public CatalogHistory GetHistory()
        {
            if (!User.CatalogHistory.ContainsKey(Shop.Id))
                User.CatalogHistory[Shop.Id] = new CatalogHistory();

            return User.CatalogHistory[Shop.Id];
        }

        public ArcadeContext Context { get; }

        public ArcadeUser User => Context.Account;

        public Shop Shop { get; }

        public Vendor Vendor { get; }

        public ItemCatalog Catalog { get; }

        public ShopState State { get; set; }

        public IUserMessage MessageReference { get; set; }

        public int PageIndex { get; set; }

        internal int PageCount { get; set; }

        public string Notice { get; set; }

        private bool CanClearNotice { get; set; }

        private bool HasUpdatedCatalog { get; set; }

        private int KnownShopCount { get; }

        // TODO: Place in ShopHelper
        public static string GetVendorText(Vendor vendor, ShopState state, string replyOverride = null)
        {
            string name = vendor?.Name ?? Vendor.NameGeneric;
            string reply = string.IsNullOrWhiteSpace(replyOverride) ? ShopHelper.GetVendorReply(vendor, state) : replyOverride;
            return $"**{name}**: \"{reply}\"";
        }

        // TODO: Place in ShopHelper
        public static string GetMenuHeader(Vendor vendor, ShopState state, string replyOverride = null)
        {
            var header = new StringBuilder();

            header.AppendLine($"> {GetVendorText(vendor, state, replyOverride)}");

            if (!state.EqualsAny(ShopState.Exit, ShopState.Timeout))
                header.AppendLine($"> {GetMenuActions(state)}");
            // Write Menu Footer (available cash for the shop's currency type, and available inventory space)
            return header.ToString();
        }

        // TODO: Make static and place in ShopHelper
        public string GetMenuBody(ArcadeUser user, Shop shop, ItemCatalog catalog, ShopState state)
        {
            var body = new StringBuilder();

            if (state.EqualsAny(ShopState.ViewBuy, ShopState.Buy))
            {
                body.Append(ShopHelper.WriteCatalog(shop.Catalog, catalog, PageIndex));

                if (!HasUpdatedCatalog)
                {
                    foreach ((string itemId, int amount) in Paginate.GroupAt(catalog.ItemIds, PageIndex, ShopHelper.GroupSize)) // catalog.ItemIds)
                    {
                        CatalogHelper.SetCatalogStatus(user, itemId, CatalogStatus.Seen);
                    }

                    HasUpdatedCatalog = true;
                }
            }
            else if (state.EqualsAny(ShopState.ViewSell, ShopState.Sell))
            {
                body.AppendLine();
                body.Append(InventoryViewer.ViewShopSellables(user, shop));
            }

            return body.ToString();
        }

        // TODO: Place in ShopHelper
        // TODO: This needs to include the `next` `previous` `first` `last` `goto <page>` base on page count
        public static string GetMenuActions(ShopState state)
        {
            // var actions = new List<string>();

            if (state.EqualsAny(ShopState.Enter, ShopState.Menu, ShopState.BuyDeny, ShopState.BuyEmpty,
                ShopState.SellDeny, ShopState.SellEmpty, ShopState.BuyRemainder))
            {
                // actions.AddRange("buy", "sell", "leave");

                return "`buy` `sell` `leave`";
            }

            return "`<item_id>` `back` `leave`";
        }

        // TODO: Place in ShopHelper
        public static string WriteBuyNotice(Item item, int amount, long cost)
        {
            var notice = new StringBuilder();
            notice.AppendLine($"> 🧾 You have purchased an item.");
            // notice.AppendLine($"> 🧾 You have purchased {(amount > 1 ? $"**{amount:##,0}**" : "an")} {Format.TryPluralize("item", amount)}.");
            notice.Append("• ");

            string icon = item.GetIcon();

            if (!string.IsNullOrWhiteSpace(icon))
                notice.Append($"{icon} ");

            notice.Append($"{(!string.IsNullOrWhiteSpace(icon) ? item.Name : item.GetName())} ... ");
            long costSum = amount > 1 ? cost * amount : cost;
            notice.Append($"{Icons.IconOf(item.Currency)} **{costSum:##,0}**\n\n");

            return notice.ToString();
        }

        // TODO: Make static and place in ShopHelper
        private string DrawMenu(ArcadeUser user, Vendor vendor, ItemCatalog catalog, Shop shop, ShopState state, string notice = "", string replyOverride = null)
        {
            var menu = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(notice))
                menu.Append(notice);

            menu.Append(GetMenuHeader(vendor, state, replyOverride));
            menu.Append(GetMenuBody(user, shop, catalog, state));

            return menu.ToString();
        }

        public override async Task OnStartAsync()
        {
            User.IsInSession = true;
            State = ShopState.Enter;
            Notice = $"> Welcome to **{Shop.Name}**.\n\n";
            MessageReference = await Context.Channel.SendMessageAsync(DrawMenu(Context.Account, Vendor, Catalog, Shop, State, Notice));
        }

        private async Task UpdateAsync(string replyOverride = null)
        {
            Console.WriteLine($"{State}");

            if (State.EqualsAny(ShopState.Exit, ShopState.Timeout))
                Notice = "";

            // If you cannot clear a notice, and the Notice is specified
            else if (!CanClearNotice && !string.IsNullOrWhiteSpace(Notice))
                CanClearNotice = true;
            else
            {
                if (!string.IsNullOrWhiteSpace(Notice))
                    Notice = "";

                CanClearNotice = false;
            }

            await MessageReference.ModifyAsync(DrawMenu(Context.Account, Vendor, Catalog, Shop, State, Notice, replyOverride)).ConfigureAwait(false);
        }

        // TODO: Separate the three primary shop menu states into their own methods to simplify this method
        public override async Task<SessionResult> OnMessageReceivedAsync(SocketMessage message)
        {
            var reader = new StringReader(message.Content);


            string command = reader.ReadUnquotedString();
            reader.SkipWhiteSpace();
            string arg = "";

            if (reader.CanRead())
            {
                arg = reader.ReadUnquotedString();
            }

            if (command.EqualsAny("buy", "sell", "leave", "back") || ItemHelper.Exists(command))
            {
                Notice = null;
                await message.TryDeleteAsync();
            }

            // If the current state is ENTER | MENU
            if (State.EqualsAny(ShopState.Enter, ShopState.Menu, ShopState.BuyDeny, ShopState.BuyEmpty, ShopState.BuyLimit, ShopState.SellDeny, ShopState.SellEmpty, ShopState.BuyRemainder))
            {
                switch (command)
                {
                    case "buy" when !Shop.Buy:
                        State = ShopState.BuyDeny;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "buy" when Catalog.ItemIds.Count == 0:
                        State = ShopState.BuyEmpty;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "buy" when Shop.MaxAllowedPurchases.HasValue && GetHistory().PurchasedIds.Values.Sum() >= Shop.MaxAllowedPurchases:
                        State = ShopState.BuyLimit;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "buy":
                        State = ShopState.ViewBuy;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "sell" when !Shop.Sell:
                        State = ShopState.SellDeny;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "sell" when User.Items.Count == 0:
                        State = ShopState.SellEmpty;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "sell":
                        if (User.Items.Count(x => Shop.AllowedSellGroups.Contains(ItemHelper.GroupOf(x.Id))) == 0)
                        {
                            State = ShopState.SellEmpty;
                            await UpdateAsync();
                            return SessionResult.Continue;
                        }

                        State = ShopState.ViewSell;
                        await UpdateAsync();
                        return SessionResult.Continue;

                        // Handle unlocking new shops
                        // by having the vendor talk about it
                        // "Hey. Word is, there's another shop out there that sells color palettes on an entirely different level."
                    case "leave":
                    case "back":


                        // TODO: Implement notifying the user if a new shop has appeared for them.
                        /*
                        string replyOverride = null;
                        if (ShopHelper.GetKnownShops(User).Count() != KnownShopCount)
                        {
                            replyOverride = "";
                        }
                        */

                        Notice = null;
                        State = ShopState.Exit;
                        await UpdateAsync();
                        return SessionResult.Success;

                    default:
                        return SessionResult.Continue;
                }
            }

            // If the current state is VIEW_BUY
            if (State.EqualsAny(ShopState.ViewBuy, ShopState.Buy, ShopState.BuyFail, ShopState.BuyInvalid))
            {
                switch (command)
                {
                    // back
                    case "back":
                        Notice = null;
                        State = ShopState.Menu;
                        await UpdateAsync();
                        return SessionResult.Continue;
                    // leave
                    case "leave":
                        Notice = null;
                        State = ShopState.Exit;
                        await UpdateAsync();
                        return SessionResult.Success;
                }

                State = ShopState.ViewBuy;

                Item item = GetItemFromCatalog(command);


                if (item == null && !ItemHelper.Exists(command))
                    return SessionResult.Continue;

                if (item == null)
                {
                    State = ShopState.BuyInvalid;
                    // Write a notice
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                int amount = ParseAmount(arg, item);

                if (amount < 1)
                {
                    await UpdateAsync("I don't think I heard that correctly. How many did you want to purchase?");
                    return SessionResult.Continue;
                }

                if (!CanBuy(item, amount))
                {
                    State = ShopState.BuyFail;
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                // Otherwise, if the item can be bought:

                // Give the item to the user
                ItemHelper.GiveItem(User, item, amount);
                long cost = GetCost(item, amount);
                // Take the money from the user
                Take(cost, item.Currency);
                // Add the money spent to the total spent count
                Var.AddToValue(User, ShopHelper.GetTotalSpentId(Shop.Id), cost);
                RemoveFromCatalog(item, amount);

                Notice = WriteBuyNotice(item, amount, GetCost(item, amount));
                CanClearNotice = false;

                if (Catalog.ItemIds.Count == 0)
                {
                    State = ShopState.BuyRemainder;
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                State = ShopState.Buy;
                await UpdateAsync();
                return SessionResult.Continue;
            }

            if (State.EqualsAny(ShopState.ViewSell, ShopState.Sell, ShopState.SellInvalid, ShopState.SellNotAllowed, ShopState.SellNotOwned))
            {
                switch (command)
                {
                    case "back":
                        State = ShopState.Menu;
                        await UpdateAsync();
                        return SessionResult.Continue;

                    case "leave":
                        State = ShopState.Exit;
                        await UpdateAsync();
                        return SessionResult.Success;
                }

                State = ShopState.ViewSell;

                Item item = GetItemFromInventory(command);

                if (item == null && !ItemHelper.Exists(command))
                    return SessionResult.Continue;

                if (item == null)
                {
                    State = ShopState.SellInvalid;
                    // Write a notice
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                if (!CanSell(item))
                {
                    State = ShopState.SellNotAllowed;
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                int amount = ParseAmount(arg, item);

                if (amount < 1)
                {
                    await UpdateAsync("I don't think I heard that correctly. How many did you want to sell?");
                    return SessionResult.Continue;
                }

                if (ItemHelper.GetOwnedAmount(User, item) == 0)
                {
                    State = ShopState.SellNotOwned;
                    await UpdateAsync();
                    return SessionResult.Continue;
                }

                // Take the item from the user
                RemoveFromInventory(item, amount);
                long worth = GetWorth(item, amount);
                User.Give(worth, item.Currency);

                State = ShopState.Sell;
                await UpdateAsync();
                return SessionResult.Continue;
            }

            // Otherwise, wait for the next input
            return SessionResult.Continue;
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            State = ShopState.Timeout;
            Notice = null;
            await UpdateAsync();
        }

        // TODO: Remove this method and use the existing ShopHelper.CanSell() method
        private bool CanSell(Item item)
            => Shop.AllowedSellGroups.Contains(item.GroupId);

        private int ParseAmount(string input, Item item)
        {
            int ownCount = ItemHelper.GetOwnedAmount(User, item);

            if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                return ownCount;

            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int amount))
                return 1;

            return Math.Clamp(amount, 0, ownCount);
        }

        private void RemoveFromInventory(Item item, int amount)
        {
            _ = GetHistory();

            ItemHelper.TakeItem(User, item, amount);

            if (!User.CatalogHistory[Shop.Id].SoldIds.TryAdd(item.Id, amount))
                User.CatalogHistory[Shop.Id].SoldIds[item.Id] += amount;

            User.AddToVar(Stats.Common.ItemsSold, amount);
            User.AddToVar(ShopHelper.GetTotalSoldId(Shop.Id), amount);
        }

        private void RemoveFromCatalog(Item item, int amount)
        {
            _ = GetHistory();

            // Remove the purchased items from the catalog
            Catalog.ItemIds[item.Id] -= amount;

            if (!User.CatalogHistory[Shop.Id].PurchasedIds.TryAdd(item.Id, amount))
                User.CatalogHistory[Shop.Id].PurchasedIds[item.Id] += amount;

            if (Catalog.ItemIds[item.Id] <= 0)
            {
                Catalog.ItemIds.Remove(item.Id);

                if (Catalog.Discounts.ContainsKey(item.Id))
                    Catalog.Discounts.Remove(item.Id);
            }

            User.AddToVar(Stats.Common.ItemsBought, amount);
            User.AddToVar(ShopHelper.GetTotalBoughtId(Shop.Id), amount);
        }

        // TODO: Move to ShopHelper
        private long GetWorth(Item item, int amount)
        {
            long value = Shop.SellDeduction > 0
                ? GetWorth(item.Value, Shop.SellDeduction)
                : item.Value;

            return value * amount;
        }

        // TODO: Move to ShopHelper
        private long GetWorth(long value, int deduction)
            => (long)Math.Floor(value * (1 - deduction / (double)100));

        // TODO: Move to ShopHelper
        private bool CanBuy(Item item, int amount)
        {
            long cost = GetCost(item, amount);
            return CanTake(cost, item.Currency);
        }

        // TODO: Move to ShopHelper
        private long GetCost(Item item, int amount)
        {
            long cost = ShopHelper.CostOf(item, Catalog);
            return cost * amount;
        }

        // TODO: Move to ShopHelper
        private bool CanTake(long value, CurrencyType currency)
        {
            long balance = GetBalance(currency);
            return balance - value >= 0;
        }

        // TODO: Move to ShopHelper
        private void Take(long value, CurrencyType currency)
        {
            if (!CanTake(value, currency))
                return;

            User.Take(value, currency);
        }

        // TODO: Move to ShopHelper
        private long GetBalance(CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.Money => User.Balance,
                CurrencyType.Chips => User.ChipBalance,
                CurrencyType.Tokens => User.TokenBalance,
                CurrencyType.Debt => User.Debt,
                _ => throw new Exception("Unknown currency")
            };
        }

        private Item GetItemFromCatalog(string itemId)
        {
            if (Catalog.ItemIds.Any(x => x.Key == itemId))
                return ItemHelper.GetItem(Catalog.ItemIds.First(x => x.Key == itemId).Key);

            return null;
        }

        private Item GetItemFromInventory(string itemId)
        {
            if (User.Items.Any(x => x.Data != null && x.Data.Id == itemId))
                return ItemHelper.ItemOf(User, itemId);

            if (User.Items.All(x => x.Id != itemId))
                return null;

            return ItemHelper.GetItem(itemId);
        }
    }
}
