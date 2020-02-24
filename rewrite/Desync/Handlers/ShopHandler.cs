using Discord;
using Discord.WebSocket;
using Orikivo.Unstable;
using System.Threading.Tasks;

using System.Collections.Generic;
using Discord.Rest;
using System.Text;
using System.Linq;
using Orikivo.Drawing;
using System;

namespace Orikivo
{
    public class ShopHandler : MatchAction
    {
        public ShopHandler(OriCommandContext context, Market market, PaletteType palette)
        {
            Context = context;
            Market = market;
            Vendor = market.GetActive();
            Palette = GraphicsService.GetPalette(palette);
        }

        // essentially required.
        public OriCommandContext Context { get; }

        private Husk Husk => Context.Account.Husk;

        // the starting message that the bot sends.
        public RestUserMessage Initial { get; private set; }

        public Market Market { get; }

        public Vendor Vendor { get; }
        public GammaPalette Palette { get; }

        public MarketState State { get; private set; }
        private MarketState LastState { get; set; }
        // the last actual transaction that was performed.
        public MarketHistory Last { get; private set; }

        // used to keep track of the item that might be sold or bought.
        private Item Confirm { get; set; }
        private int ConfirmMaxCount { get; set; }
        private int ConfirmCount { get; set; }

        // the item to inspect when viewing stuff.
        private Item Inspector { get; set; }

        // TODO: Implement MarketDialogue
        // to customize vendor speech.
        private string GetNextResponse()
        {
            switch(State)
            {
                case MarketState.Menu:
                    if (LastState == State)
                        return "Welcome";
                    return "Anything else I can do for you?";

                case MarketState.Buy:
                    if (!CanBuy())
                        return "It seems I've run out of items to sell you.";
                    return "What would you like to buy?";

                case MarketState.BuyConfirm:
                    return "How many would you like to buy?";

                case MarketState.BuyComplete:
                    if (!CanBuy())
                        return "You just bought the last of the items I had in stock. Thank you.";
                    return "Thank you for the purchase.";

                case MarketState.Sell:
                    return "What would you like to sell?";

                case MarketState.SellConfirm:
                    return "How many would you like to sell?";

                case MarketState.SellRevert:
                    return "Sorry about that. Must've been a mistake.";

                case MarketState.SellComplete:
                    return "Here you are.";

                default:
                    return "INVALID_STATE_TRANSITION";
            }
        }

        // check if they can afford this

        private bool CanStore(Item item, int amount = 1)
            => (Husk.Backpack.ItemIds.ContainsKey(item.Id) ? Husk.Backpack.ItemIds.Count : Husk.Backpack.ItemIds.Count + amount)
            <= Husk.Backpack.Capacity;

        private bool CanAfford(ulong value)
            => ((long)Context.Account.Balance - (long)value) >= 0;

        private bool CanSell()
            => Context.Account.Husk.Backpack.ItemIds.Count > 0;

        // shows your current money and backpack slots.
        private string GetMenuFooter()
        {
            StringBuilder footer = new StringBuilder();

            footer.Append(WriteValue(Context.Account.Balance));
            int capacity = Husk.Backpack.Capacity;
            footer.Append($" ({capacity - Husk.Backpack.ItemIds.Count}/{capacity} {OriFormat.GetNounForm("Slot", capacity)} Available)");

            return footer.ToString();
        }

        private ulong GetItemValue(Item item, bool isSellable = false)
            => isSellable ? (ulong)Math.Floor(item.Value * Market.SellRate) : item.Value;

        private string GetBackpackContent()
        {
            StringBuilder bp = new StringBuilder();

            bp.AppendLine("**Backpack**");

            if (Husk.Backpack.ItemIds.Count == 0)
                bp.Append("> **Your Backpack is empty.**");
            else
                bp.AppendJoin("\n", Husk.Backpack.ItemIds.Select(x => GetItemSummary(WorldEngine.GetItem(x.Key), x.Value, true)));

            return bp.ToString();
        }

        private string GetItemSummary(Item item, int amount = 1, bool isSellable = false)
        {
            bool isSoldOut = amount == 0 && !isSellable;

            StringBuilder summary = new StringBuilder();
            summary.Append("> ");

            if (isSoldOut)
                summary.Append("~~");

            summary.Append($"[`{item.Id}`] ");
            summary.Append(isSoldOut ? "*" : "**");
            summary.Append(item.Name);
            summary.Append(isSoldOut ? "*~~ " : "** ");

            if (isSoldOut)
            {
                summary.Append("• **(Sold Out)**");
                return summary.ToString();
            }
            
            if (amount > 1 && isSellable)
                summary.Append($"(x{OriFormat.Notate(amount)}) ");

            summary.Append($"• {WriteValue(GetItemValue(item, isSellable))}");

            if (!isSellable)
                summary.Append($" (**{OriFormat.Notate(amount)} in stock**)");

            return summary.ToString();
        }

        private string GetReply(string response)
            => $"**{Vendor.Name}**: {response}";

        private string GetCurrentMenu(MarketState state, string reply = null)
        {
            StringBuilder menu = new StringBuilder();

            LastState = State;
            State = state;

            if (State != MarketState.View)
                menu.AppendLine(GetReply(Check.NotNull(reply) ? reply : GetNextResponse()));

            menu.AppendLine(GetMenuContent());
            menu.Append(GetMenuFooter());

            return menu.ToString();
        }

        private CatalogData GetCatalog()
            => Context.Account.Brain.GetOrGenerateCatalog(Market);

        // determines if there are any items left to buy.
        private bool CanBuy()
            => (GetCatalog().Count > 0);

        private string GetCatalogContent()
        {
            // TODO: implement cost value based on coupons and discounts, etc.
            StringBuilder catalog = new StringBuilder();
            catalog.AppendLine("**Catalog**");
            catalog.AppendJoin("\n", GetCatalog().Decompress().Items.Select(x => GetItemSummary(x.Key, x.Value)));
            return catalog.ToString();
        }

        private string GetMenuContent()
        {
            StringBuilder content = new StringBuilder();

            switch (State)
            {
                case MarketState.Menu:
                    content.AppendLine(GetActions(view: false, buy: Market.CanBuyFrom, sell: Market.CanSellFrom));
                    break;

                case MarketState.View:
                    content.AppendLine(GetItemDetails(Inspector));
                    content.AppendLine();
                    content.Append(GetActions(view: false));
                    break;

                // if in the buy menu, show the catalog of available items.
                case MarketState.Buy:
                case MarketState.BuyConfirm:
                    content.AppendLine(GetCatalogContent());
                    content.AppendLine();
                    content.Append(GetActions());
                    break;

                // if you purchased an item, include what you bought ABOVE the catalog of available items
                case MarketState.BuyComplete:
                    content.AppendLine(GetTransaction());
                    content.AppendLine(GetCatalogContent());
                    content.AppendLine();
                    content.Append(GetActions());
                    break;

                // if in the sell menu, show the contents of your backpack
                case MarketState.Sell:
                case MarketState.SellConfirm:
                    content.AppendLine(GetBackpackContent());
                    content.AppendLine();
                    content.Append(GetActions());
                    break;

                // if you sold an item, include what you sold ABOVE the contents of your backpack
                case MarketState.SellComplete:
                    content.AppendLine(GetTransaction());
                    content.AppendLine(GetBackpackContent());
                    content.AppendLine();
                    content.Append(GetActions(true));
                    break;

                // if you reverted a sell, let them know they did ABOVE the contents of your backpack.
                case MarketState.SellRevert:
                    content.AppendLine(GetTransaction());
                    content.AppendLine(GetBackpackContent());
                    content.AppendLine();
                    content.Append(GetActions());
                    break;
            }

            return content.ToString();
        }

        private string GetTransaction()
        {
            string money = WriteValue(Last.Value);
            string item = $"**{OriFormat.Notate(Last.Count)}** {OriFormat.GetNounForm("item", Last.Count)}";

            return State switch
            {
                MarketState.BuyComplete => $"Bought {item} for {money}.",
                MarketState.SellComplete => $"Sold {item} for {money}.",
                MarketState.SellRevert => $"Returned {money} for {item}.",
                _ => "INVALID_TRANSACTION"
            };
        }

        private string WriteValue(ulong value)
            => $"**💸{OriFormat.Notate(value)}**";
        private string GetItemDetails(Item item)
        {
            StringBuilder details = new StringBuilder();

            details.AppendLine($"**{item.Name}** • {WriteValue(Last.Value)}");

            if (Check.NotNullOrEmpty(item.Quotes))
                details.AppendLine($"> *\"{Randomizer.Choose(item.Quotes)}\"*");

            details.Append("**#**");
            details.AppendJoin(", ", EnumUtils.GetFlags(item.Tag).Select(x => $"`{x.ToString()}`"));

            if (Check.NotNull(item.Summary))
            {
                details.AppendLine();
                details.Append($"```{item.Summary}```");
            }

            return details.ToString();
        }

        // TODO: implement talk sub-handling.
        private string GetActions(bool revert = false, bool view = true, bool buy = false, bool sell = false, bool back = true)
        {
            List<string> valid = new List<string>();

            if (buy)
                valid.Add("buy");

            if (sell)
                valid.Add("sell");

            if (view)
                valid.Add("view");

            if (revert)
                valid.Add("revert");

            if (back)
                valid.Add("back");

            return "> " + string.Join(" • ", valid.Select(x => $"`{x}`"));
        }

        public override async Task OnStartAsync()
        {
            State = MarketState.Menu;
            
            if (Vendor.Sheet != null)
                Initial = await Context.Channel.SendImageAsync(Vendor.Sheet.GetDisplayImage(DialogueTone.Neutral, Palette), "../tmp/npc.png",
                    GetCurrentMenu(MarketState.Menu));
            else
                Initial = await Context.Channel.SendMessageAsync(GetCurrentMenu(MarketState.Menu));
        }

        // handle what the next market action is...?

        private Trigger ParseMessage(SocketMessage message, string separator = " ")
        {
            if (!Check.NotNull(message.Content))
                return new Trigger { IsSuccess = false };

            string[] args = message.Content.Split(separator);

            return new Trigger
            {
                Name = args[0],
                Args = args.Skip(1).ToArray(),
                IsSuccess = true
            };
        }

        private bool TryGetFromBackpack(string itemId, out Item item)
        {
            item = null;
            bool result = Husk.Backpack.ItemIds.ContainsKey(itemId);

            if (result)
                item = WorldEngine.GetItem(itemId);

            return result;
        }

        private bool TryGetFromCatalog(string itemId, out Item item)
        {
            item = null;
            bool result = GetCatalog().ItemIds.ContainsKey(itemId);

            if (result)
                item = WorldEngine.GetItem(itemId);

            return result;
        }

        public override async Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            Trigger trigger = ParseMessage(message);
            await message.DeleteAsync();

            // allow arguments to stack

            // buy 
            if (State.EqualsAny(MarketState.Buy, MarketState.BuyComplete))
            {
                if (trigger.Name == "view")
                {
                    if (trigger.ArgCount != 1)
                    {
                        await UpdateAsync(MarketState.Buy, "You must specify a single item ID.");
                        return ActionResult.Continue;
                    }

                    string id = trigger.Args[0];

                    if (GetCatalog().ItemIds.ContainsKey(id))
                    {
                        Inspector = WorldEngine.GetItem(id);
                        await UpdateAsync(MarketState.View);
                        return ActionResult.Continue;
                    }
                    else
                    {
                        await UpdateAsync(MarketState.Buy, "This item could not be found.");
                        return ActionResult.Continue;
                    }
                }

                if (TryGetFromCatalog(trigger.Name, out Item toConfirm))
                {
                    int count = GetCatalog().ItemIds[trigger.Name];

                    if (count <= 0)
                    {
                        await UpdateAsync(MarketState.Buy, "It seems we've run out of this item, sorry.");
                        return ActionResult.Continue;
                    }

                    if (!CanAfford(toConfirm.Value))
                    {
                        await UpdateAsync(MarketState.Buy, "You can't afford this, sorry.");
                        return ActionResult.Continue;
                    }

                    if (!CanStore(toConfirm, count))
                    {
                        await UpdateAsync(MarketState.Buy, "You can't store anything else.");
                        return ActionResult.Continue;
                    }


                    if (count > 1)
                    {
                        Confirm = toConfirm;
                        ConfirmMaxCount = count;
                        await UpdateAsync(MarketState.BuyConfirm);
                        return ActionResult.Continue;
                    }

                    Last = new MarketHistory(new Dictionary<string, int> { [toConfirm.Id] = 1 }, toConfirm.Value);


                    Last.ApplyBuy(Context.Account, Market.Id);
                    await UpdateAsync(MarketState.BuyComplete);
                    return ActionResult.Continue;

                }
                // item_id
                // amount OR all
            }

            if (State.EqualsAny(MarketState.Sell, MarketState.SellRevert, MarketState.SellComplete))
            {
                if (trigger.Name == "view")
                {
                    if (trigger.ArgCount != 1)
                    {
                        await UpdateAsync(State, "You must specify a single item ID.");
                        return ActionResult.Continue;
                    }

                    string id = trigger.Args[0];

                    if (Husk.Backpack.ItemIds.ContainsKey(id))
                    {
                        Inspector = WorldEngine.GetItem(id);
                        await UpdateAsync(MarketState.View);
                        return ActionResult.Continue;
                    }
                    else
                    {
                        await UpdateAsync(MarketState.Sell, "This item could not be found.");
                        return ActionResult.Continue;
                    }
                }

                if (TryGetFromBackpack(trigger.Name, out Item toConfirm))
                {
                    int count = Husk.Backpack.ItemIds[trigger.Name];

                    if (count > 1)
                    {
                        Confirm = toConfirm;
                        ConfirmMaxCount = count;
                        await UpdateAsync(MarketState.SellConfirm);
                        return ActionResult.Continue;
                    }

                    Last = new MarketHistory(new Dictionary<string, int> { [toConfirm.Id] = 1 }, GetItemValue(toConfirm));

                    Last.ApplySell(Context.Account);
                    await UpdateAsync(MarketState.SellComplete);
                    return ActionResult.Continue;
                }
            }

            // sell
            if (State == MarketState.Menu)
            {
                if (trigger.Name == "buy")
                {
                    if (!Market.CanBuyFrom)
                        await UpdateAsync(MarketState.Menu, "Sorry, but I don't sell items here.");
                    else if (!CanBuy())
                        await UpdateAsync(MarketState.Menu, "There isn't anything left to buy.");
                    else
                        await UpdateAsync(MarketState.Buy);

                    return ActionResult.Continue;
                }

                if (trigger.Name == "sell")
                {
                    if (!Market.CanSellFrom)
                    {
                        await UpdateAsync(MarketState.Menu, "Sorry, but I don't buy items here.");
                        return ActionResult.Continue;
                    }


                    if (!CanSell())
                    {
                        await UpdateAsync(MarketState.Menu, "You don't have anything to sell.");
                        return ActionResult.Continue;
                    }
                    await UpdateAsync(MarketState.Sell);
                    return ActionResult.Continue;
                }
            }

            // item_id
            // amount OR all
            // revert

            if (State == MarketState.BuyConfirm)
            {
                bool valid = false;
                if (trigger.Name == "view")
                {
                    Inspector = Confirm;
                    await UpdateAsync(MarketState.View);
                    return ActionResult.Continue;
                }

                if (trigger.Name == "all")
                {
                    valid = true;
                    ConfirmCount = ConfirmMaxCount;
                }
                else if (int.TryParse(trigger.Name, out int amount))
                {
                    // TODO: Handle going over the max count.
                    if (amount > ConfirmCount)
                        ConfirmCount = ConfirmMaxCount;
                    ConfirmCount = amount;
                    valid = true;
                }

                if (valid)
                {
                    Last = new MarketHistory(new Dictionary<string, int> { [Confirm.Id] = ConfirmCount },
                        Confirm.Value * (ulong)ConfirmCount);

                    if (!CanAfford(Last.Value))
                    {
                        await UpdateAsync(MarketState.Buy, "You can't afford this, sorry.");
                        Confirm = null;
                        ConfirmCount = 0;
                        ConfirmMaxCount = 0;
                        return ActionResult.Continue;
                    }

                    if (!CanStore(Confirm, ConfirmCount))
                    {
                        await UpdateAsync(MarketState.Buy, "You can't store anything else.");
                        Confirm = null;
                        ConfirmCount = 0;
                        ConfirmMaxCount = 0;
                        return ActionResult.Continue;
                    }

                    Last.ApplyBuy(Context.Account, Market.Id);
                    await UpdateAsync(MarketState.BuyComplete);
                    Confirm = null;
                    ConfirmCount = 0;
                    ConfirmMaxCount = 0;
                    return ActionResult.Continue;
                }

                if (trigger.Name == "back")
                {
                    Confirm = null;
                    ConfirmCount = 0;
                    ConfirmMaxCount = 0;
                }
            }

            // specifying sell amount
            if (State == MarketState.SellConfirm)
            {
                bool valid = false;
                if (trigger.Name == "view")
                {
                    Inspector = Confirm;
                    await UpdateAsync(MarketState.View);
                    return ActionResult.Continue;
                }
                if (trigger.Name == "all")
                {
                    valid = true;
                    ConfirmCount = ConfirmMaxCount;
                }
                else if (int.TryParse(trigger.Name, out int amount))
                {
                    valid = true;
                    // TODO: Handle going over the max count.
                    if (amount > ConfirmCount)
                        ConfirmCount = ConfirmMaxCount;
                    ConfirmCount = amount;
                }

                if (valid)
                {
                    Last = new MarketHistory(new Dictionary<string, int> { [Confirm.Id] = ConfirmCount },
                        (ulong)Math.Floor(Confirm.Value * Market.SellRate) * (ulong)ConfirmCount);

                    Last.ApplySell(Context.Account);
                    
                    await UpdateAsync(MarketState.SellComplete);
                    Confirm = null;
                    ConfirmCount = 0;
                    ConfirmMaxCount = 0;
                    return ActionResult.Continue;
                    // make history info
                }

                if (trigger.Name == "back")
                {
                    Confirm = null;
                    ConfirmCount = 0;
                    ConfirmMaxCount = 0;
                }
            }

            // reverting a sale.
            if (State == MarketState.SellComplete)
            {
                if (trigger.Name == "revert")
                {
                    Last.ApplyBuy(Context.Account);

                    await UpdateAsync(MarketState.SellRevert);
                    Last = null;
                    return ActionResult.Continue;
                    // revert and delete history.
                }
            }

            // this one is always allowed
            if (trigger.Name == "back")
            {
                switch (State)
                {
                    case MarketState.Menu:
                        await Initial.ModifyAsync(GetReply("See you next time."));
                        return ActionResult.Success;

                    case MarketState.View:
                        Inspector = null;
                        await UpdateAsync(LastState);
                        return ActionResult.Continue;

                    default:
                        await UpdateAsync(MarketState.Menu);
                        return ActionResult.Continue;
                }
            }

            // on an invalid action
            return ActionResult.Continue;
        }

        private async Task UpdateAsync(MarketState state, string reply = null)
            => await Initial.ModifyAsync(x => x.Content = GetCurrentMenu(state, reply));

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            await Initial.ModifyAsync(x =>
                x.Content = GetReply("Sorry, but I have other customers I need to take care of. Try coming back whenever you're ready."));
        }
    }
}
