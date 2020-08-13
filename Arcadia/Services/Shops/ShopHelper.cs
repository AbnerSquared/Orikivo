using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Desync;

namespace Arcadia
{
    public static class ShopHelper
    {
        public static readonly List<Shop> Shops =
            new List<Shop>
            {
                new Shop
                {
                    Id = "backlog",
                    Name = "Backlogs",
                    Quote = "The shop that collects colorful goods.",
                    Vendors = new List<Vendor>
                    {
                        //Name = "V3-NDR"
                    },
                    Catalog = new CatalogGenerator
                    {
                        Size = 2,
                        MaxDiscountsAllowed = 1,
                        MaxSpecialsAllowed = 0,
                        Entries = new List<CatalogEntry>
                        {
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteGammaGreen,
                                Weight = 99,
                                MaxDiscount = 10
                            },
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteWumpite,
                                Weight = 15,
                                MaxDiscount = 5
                            },
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteCrimson,
                                Weight = 35,
                                MaxDiscount = 10
                            },
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteGlass,
                                Weight = 2,
                                MaxAllowed = 1,
                                IsSpecial = true
                            }
                        }
                    }
                }
            };

        public static Shop GetShop(string id)
        {
            if (Shops.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one shops with the specified ID.");

            return Shops.FirstOrDefault(x => x.Id == id);
        }

        public static ItemCatalog CatalogOf(string shopId)
        {
            Shop shop = GetShop(shopId);

            return shop?.Catalog.Generate();
        }

        // This writes the catalog info
        public static string WriteCatalog(ItemCatalog catalog)
        {
            var info = new StringBuilder();


            return info.ToString();
        }

        public static string NameOf(string shopId)
            => GetShop(shopId).Name;
    }

    public enum ShopState
    {
        Enter = 1,
        Menu = 2,
        Buy = 3,
        Sell = 4,
        Timeout = 5,
        ViewBuy = 6,
        ViewSell = 7,
        Exit = 8
    }

    public class ShopHandler : MatchAction
    {
        public ShopHandler(ArcadeContext context, Shop shop)
        {
            Context = context;
            Shop = shop;
            Vendor = Check.NotNullOrEmpty(shop.Vendors) ? Randomizer.Choose(shop.Vendors) : null;
            Catalog = shop.Catalog.Generate();
        }

        public ArcadeContext Context { get; }
        public Shop Shop { get; }
        public Vendor Vendor { get; }
        public ItemCatalog Catalog { get; }

        public ShopState State { get; set; }

        public IUserMessage MessageReference { get; set; }

        public long CostOf(Item item)
        {
            long value = item.Value;

            if (Catalog.Discounts.ContainsKey(item.Id))
            {
                // 1 - (20 / 100) => .2 => .8
                float discount = Catalog.Discounts[item.Id] / (float) 100;

                value *= (long) (1 - discount);
            }

            return value;
        }

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

        public string WriteVendor(Vendor vendor, ShopState state)
        {
            string name = vendor?.Name ?? Vendor.NameGeneric;
            string reply = GetVendorReply(vendor, state);
            return $"**{name}**: *\"{reply}\"*";
        }

        public string WriteMenuHeader()
        {
            var header = new StringBuilder();
            string vendor = WriteVendor(Vendor, State);
            string actions = WriteActionBar(State);

            header.AppendLine(vendor);
            header.AppendLine(actions);

            return header.ToString();
        }

        public string WriteMenuBody()
        {
            var body = new StringBuilder();
            // Menu, Enter, Exit, Timeout: These do not have bodies
            // ViewBuy: WriteCatalog()
            // ViewSell: Inventory.Write()

            return body.ToString();
        }

        private static readonly string OnTryBuy = "I don't sell items.";
        private static readonly string OnBuyEmpty = "I don't have anything else I can offer you.";
        private static readonly string OnTrySell = "I don't buy items.";
        private static readonly string OnSellEmpty = "You don't have anything I can buy from you.";

        public static string WriteActionBar(ShopState state)
        {
            var bar = new StringBuilder();

            // Enter: buy sell leave
            // Menu: buy sell leave
            // ViewBuy: <item_id> back leave
            // Buy => Points to Menu or ViewBuy
            // ViewSell: <item_id> back leave
            // Sell => Points to ViewSell or Menu


            return bar.ToString();
        }

        public string WriteEntry(Item item, int amount)
        {
            if (amount == 0)
                return "";

            var entry = new StringBuilder();

            entry.Append($"> `{item.Id}` **{item.Name}**");

            if (amount > 1)
                entry.Append($" (x**{amount:##,0}**)");

            entry.AppendLine();

            entry.AppendLine($"> *\"{item.GetQuote()}\"*");
            entry.AppendLine($"> 💸 **{CostOf(item):##,0}**  • *{item.Rarity}*");

            return entry.ToString();
        }


        public override async Task OnStartAsync()
        {

        }

        public override Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }

        public override Task OnTimeoutAsync(SocketMessage message)
        {
            throw new NotImplementedException();
        }
    }
}