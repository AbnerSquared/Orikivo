using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Orikivo;
using Format = Orikivo.Format;

namespace Arcadia
{
    public class ShopHandler : MatchSession
    {
        public ShopHandler(ArcadeContext context, Shop shop)
        {
            Context = context;
            Shop = shop;
            State = ShopState.Enter;
            Catalog = context.Data.Data.GetOrGenerateCatalog(shop, context.Account);
            List<Vendor> possibleVendors = ShopHelper.GetVendors(ShopHelper.GetUniqueTags(Catalog)).ToList();
            Vendor = Check.NotNullOrEmpty(possibleVendors) ? Randomizer.Choose(possibleVendors) : null;
        }

        public ArcadeContext Context { get; }
        public ArcadeUser User => Context.Account;
        public Shop Shop { get; }
        public Vendor Vendor { get; }
        public ItemCatalog Catalog { get; }

        public ShopState State { get; set; }

        public IUserMessage MessageReference { get; set; }

        public string Notice { get; set; }

        private bool CanClearNotice { get; set; } = false;

        public long CostOf(Item item)
            => ShopHelper.CostOf(item, Catalog);

        private static string GetGenericReply(ShopState state)
        {
            return state switch
            {
                ShopState.Enter => Vendor.EnterGeneric,
                ShopState.Buy => Vendor.BuyGeneric,
                ShopState.ViewBuy => Vendor.ViewBuyGeneric,
                ShopState.Sell => Vendor.SellGeneric,
                ShopState.ViewSell => Vendor.ViewSellGeneric,
                ShopState.Exit => Vendor.ExitGeneric,
                ShopState.Timeout => Vendor.TimeoutGeneric,
                ShopState.Menu => Vendor.MenuGeneric,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }

        private static string GetVendorReply(Vendor vendor, ShopState state)
        {
            var replies = state switch
            {
                ShopState.Enter => vendor?.OnEnter,
                ShopState.Buy => vendor?.OnBuy,
                ShopState.ViewBuy => vendor?.OnViewBuy,
                ShopState.Sell => vendor?.OnSell,
                ShopState.ViewSell => vendor?.OnViewSell,
                ShopState.Exit => vendor?.OnExit,
                ShopState.Timeout => vendor?.OnTimeout,
                ShopState.Menu => vendor?.OnMenu,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

            return Check.NotNullOrEmpty(replies) ? Randomizer.Choose(replies) : GetGenericReply(state);
        }

        public static string WriteVendor(Vendor vendor, ShopState state, string replyOverride = null)
        {
            string name = vendor?.Name ?? Vendor.NameGeneric;
            string reply = string.IsNullOrWhiteSpace(replyOverride) ? GetVendorReply(vendor, state) : replyOverride;
            return $"**{name}**: \"{reply}\"";
        }

        public static string WriteMenuHeader(Vendor vendor, ShopState state, string replyOverride = null)
        {
            var header = new StringBuilder();

            header.AppendLine($"> {WriteVendor(vendor, state, replyOverride)}");

            if (!state.EqualsAny(ShopState.Exit, ShopState.Timeout))
                header.AppendLine($"> {WriteActionBar(state)}");
            // Write Menu Footer (available cash for the shop's currency type, and available inventory space)
            return header.ToString();
        }

        private bool HasUpdatedCatalog = false;

        public string WriteMenuBody(ArcadeUser user, Shop shop, ItemCatalog catalog, ShopState state)
        {
            var body = new StringBuilder();

            if (state.EqualsAny(ShopState.ViewBuy, ShopState.Buy))
            {
                body.Append(ShopHelper.WriteCatalog(shop.Catalog, catalog));

                // TODO: Instead of updating the entire catalog for the user, update based on how many items are visible on that page, so that each page has to been seen to know about the item
                if (!HasUpdatedCatalog)
                {
                    foreach ((string itemId, int amount) in catalog.ItemIds)
                    {
                        ItemHelper.SetCatalogStatus(user, itemId, CatalogStatus.Seen);
                    }

                    HasUpdatedCatalog = true;
                }
            }
            else if (state.EqualsAny(ShopState.ViewSell, ShopState.Sell))
            {
                body.AppendLine();
                body.Append(Inventory.WriteItems(user, shop));
            }

            // Menu, Enter, Exit, Timeout: These do not have bodies
            // ViewBuy: WriteCatalog()
            // ViewSell: Inventory.Write()

            return body.ToString();
        }

        // TODO: Add these responses to vendors by default
        private static readonly string OnTryBuy = "I don't sell items.";
        private static readonly string OnBuyEmpty = "I don't have anything else I can offer you.";
        private static readonly string OnBuyFail = "You can't afford to pay for all of this.";
        private static readonly string OnTrySell = "I don't buy items.";
        private static readonly string OnSellEmpty = "You don't have anything I can buy from you.";

        public static string WriteActionBar(ShopState state)
        {
            return state switch
            {
                ShopState.Enter => "`buy` `sell` `leave`",
                ShopState.Menu => "`buy` `sell` `leave`",
                ShopState.ViewBuy => "`<item_id>` `back` `leave`",
                ShopState.Buy => "`<item_id>` `back` `leave`",
                ShopState.ViewSell => "`<item_id>` `back` `leave`",
                ShopState.Sell => "`<item_id>` `back` `leave`",
                _ => "UNHANDLED_STATE"
            };
        }

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

        private string GetMenu(ArcadeUser user, Vendor vendor, ItemCatalog catalog, Shop shop, ShopState state, string notice = "", string replyOverride = null)
        {
            var menu = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(notice))
                menu.Append(notice);

            menu.Append(WriteMenuHeader(vendor, state, replyOverride));
            menu.Append(WriteMenuBody(user, shop, catalog, state));

            return menu.ToString();
        }

        public override async Task OnStartAsync()
        {
            User.CanShop = false;
            State = ShopState.Enter;
            Notice = $"> Welcome to **{Shop.Name}**.\n\n";
            MessageReference = await Context.Channel.SendMessageAsync(GetMenu(Context.Account, Vendor, Catalog, Shop, State, Notice));
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

            await MessageReference.ModifyAsync(GetMenu(Context.Account, Vendor, Catalog, Shop, State, Notice, replyOverride)).ConfigureAwait(false);
        }

        public override async Task<ActionResult> InvokeAsync(SocketMessage message)
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
            if (State.EqualsAny(ShopState.Enter, ShopState.Menu))
            {
                switch (command)
                {
                    case "buy" when !Shop.Buy:
                        await UpdateAsync(OnTryBuy);
                        return ActionResult.Continue;

                    case "buy" when Catalog.ItemIds.Count == 0:
                        await UpdateAsync(OnBuyEmpty);
                        return ActionResult.Continue;

                    case "buy":
                        State = ShopState.ViewBuy;
                        await UpdateAsync();
                        return ActionResult.Continue;

                    case "sell" when !Shop.Sell:
                        await UpdateAsync(OnTrySell);
                        return ActionResult.Continue;

                    case "sell" when User.Items.Count == 0:
                        await UpdateAsync(OnSellEmpty);
                        return ActionResult.Continue;

                    case "sell":
                        if (User.Items.Count(x => (ItemHelper.GetTag(x.Id) & Shop.SellTags) != 0) == 0)
                        {
                            await UpdateAsync("You don't have any items I can buy from you.");
                            return ActionResult.Continue;
                        }

                        State = ShopState.ViewSell;
                        await UpdateAsync();
                        return ActionResult.Continue;

                        // Handle unlocking new shops
                        // by having the vendor talk about it
                        // "Hey. Word is, there's another shop out there that sells color palettes on an entirely different level."
                    case "leave":
                    case "back":
                        Notice = null;
                        State = ShopState.Exit;
                        await UpdateAsync();
                        return ActionResult.Success;

                    default:
                        Console.WriteLine("No valid command entered, returning...");
                        return ActionResult.Continue;
                }
            }

            // If the current state is VIEW_BUY
            if (State.EqualsAny(ShopState.ViewBuy, ShopState.Buy))
            {
                switch (command)
                {
                    // back
                    case "back":
                        Notice = null;
                        State = ShopState.Menu;
                        await UpdateAsync();
                        return ActionResult.Continue;
                    // leave
                    case "leave":
                        Notice = null;
                        State = ShopState.Exit;
                        await UpdateAsync();
                        return ActionResult.Success;
                }

                State = ShopState.ViewBuy;

                Item item = GetItemFromCatalog(command);


                if (item == null && !ItemHelper.Exists(command))
                    return ActionResult.Continue;

                if (item == null)
                {
                    // Write a notice
                    await UpdateAsync("I'm not sure what you're trying to buy.");
                    return ActionResult.Continue;
                }

                int amount = ParseAmount(arg, item);

                if (amount < 1)
                {
                    await UpdateAsync("How many? I don't think I heard that correctly.");
                    return ActionResult.Continue;
                }

                if (!CanBuy(item, amount))
                {
                    await UpdateAsync(OnBuyFail);
                    return ActionResult.Continue;
                }

                // Otherwise, if the item can be bought:

                // Give the item to the user
                ItemHelper.GiveItem(User, item, amount);

                // Take the money from the user
                Take(GetCost(item, amount), item.Currency);
                RemoveFromCatalog(item, amount);

                Notice = WriteBuyNotice(item, amount, GetCost(item, amount));
                CanClearNotice = false;

                if (Catalog.ItemIds.Count == 0)
                {
                    State = ShopState.Menu;
                    await UpdateAsync("You've bought everything in stock. Thank you.");
                    return ActionResult.Continue;
                }

                State = ShopState.Buy;
                await UpdateAsync();
                return ActionResult.Continue;
            }

            if (State.EqualsAny(ShopState.ViewSell, ShopState.Sell))
            {
                switch (command)
                {
                    case "back":
                        State = ShopState.Menu;
                        await UpdateAsync();
                        return ActionResult.Continue;

                    case "leave":
                        State = ShopState.Exit;
                        await UpdateAsync();
                        return ActionResult.Success;
                }

                State = ShopState.ViewSell;

                Item item = GetItemFromInventory(command);

                if (item == null && !ItemHelper.Exists(command))
                    return ActionResult.Continue;

                if (item == null)
                {
                    // Write a notice
                    await UpdateAsync("I'm not sure what you're trying to sell to me.");
                    return ActionResult.Continue;
                }

                if (!CanSell(item))
                {
                    await UpdateAsync("Sorry, but I don't buy that here.");
                    return ActionResult.Continue;
                }

                int amount = ParseAmount(arg, item);

                if (amount < 1)
                {
                    await UpdateAsync("How many? I don't think I heard that correctly.");
                    return ActionResult.Continue;
                }

                if (ItemHelper.GetOwnedAmount(User, item) == 0)
                {
                    await UpdateAsync("You don't have this item in your inventory.");
                    return ActionResult.Continue;
                }

                // Take the item from the user
                ItemHelper.TakeItem(User, item, amount);
                long worth = GetWorth(item, amount);

                Give(worth, item.Currency);

                State = ShopState.Sell;
                await UpdateAsync();
                return ActionResult.Continue;
            }

            // Otherwise, wait for the next input
            return ActionResult.Continue;
        }

        private bool CanSell(Item item)
            => (item.Tag & Shop.SellTags) != 0;

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
            if (!User.CatalogHistory.ContainsKey(Shop.Id))
                User.CatalogHistory[Shop.Id] = new CatalogHistory();

            ItemHelper.TakeItem(User, item, amount);

            if (!User.CatalogHistory[Shop.Id].SoldIds.TryAdd(item.Id, amount))
                User.CatalogHistory[Shop.Id].SoldIds[item.Id] += amount;
        }

        private void RemoveFromCatalog(Item item, int amount)
        {
            if (!User.CatalogHistory.ContainsKey(Shop.Id))
                User.CatalogHistory[Shop.Id] = new CatalogHistory();

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
        }

        private long GetWorth(Item item, int amount)
        {
            long value = Shop.SellDeduction > 0
                ? GetWorth(item.Value, Shop.SellDeduction)
                : item.Value;

            return value * amount;
        }

        private long GetWorth(long value, int deduction)
            => (long)Math.Floor(value * (1 - deduction / (double)100));

        private bool CanBuy(Item item, int amount)
        {
            long cost = GetCost(item, amount);
            return CanTake(cost, item.Currency);
        }

        private long GetCost(Item item, int amount)
        {
            long cost = ShopHelper.CostOf(item, Catalog);
            return cost * amount;
        }

        private bool CanTake(long value, CurrencyType currency)
        {
            long balance = GetBalance(currency);
            return balance - value >= 0;
        }

        private void Take(long value, CurrencyType currency)
        {
            if (!CanTake(value, currency))
                return;

            if (currency == CurrencyType.Money)
                User.Take(value, false);
            else if (currency == CurrencyType.Chips)
                User.ChipBalance -= value;
            else if (currency == CurrencyType.Tokens)
                User.TokenBalance -= value;
            else if (currency == CurrencyType.Debt)
                User.Give(value, false);
        }

        private void Give(long value, CurrencyType currency)
        {
            if (currency.HasFlag(CurrencyType.Money))
                User.Give(value, false);
            else if (currency.HasFlag(CurrencyType.Chips))
                User.ChipBalance += value;
            else if (currency.HasFlag(CurrencyType.Tokens))
                User.TokenBalance += value;
            else if (currency.HasFlag(CurrencyType.Debt))
                User.Take(value, false);
            else
                throw new Exception("Unknown currency");
        }

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

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            State = ShopState.Timeout;
            Notice = null;
            await UpdateAsync();
        }
    }
}
