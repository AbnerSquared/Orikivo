using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia
{
    public enum ShopMode
    {
        Buy = 1,
        Sell = 2
    }

    public static class ShopHelper
    {
        public static readonly List<Shop> Shops =
            new List<Shop>
            {
                new Shop
                {
                    Id = "chrome_cove",
                    Name = "Chromatic Cove",
                    Quote = "The reliable place to purchase color palettes.", // The shop that collects colorful goods.
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
                                MinDiscount = 5,
                                MaxDiscount = 10,
                                DiscountChance = 0.5f
                            },
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteWumpite,
                                Weight = 15,
                                MinDiscount = 1,
                                MaxDiscount = 5,
                                DiscountChance = 0.3f
                            },
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteCrimson,
                                Weight = 35,
                                MinDiscount = 5,
                                MaxDiscount = 10,
                                DiscountChance = 0.4f
                            },
                            new CatalogEntry
                            {
                                ItemId = Items.PaletteGlass,
                                Weight = 2,
                                MaxAllowed = 1,
                                IsSpecial = true
                            }
                        }
                    },
                    Allow = ShopAllow.All,
                    SellDeduction = 50,
                    SellTags = ItemTag.Palette
                }
            };

        public static bool Exists(string shopId)
            => Shops.Any(x => x.Id == shopId);

        public static Shop GetShop(string id)
        {
            if (Shops.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one shops with the specified ID.");

            return Shops.FirstOrDefault(x => x.Id == id);
        }

        public static ItemCatalog GenerateCatalog(string shopId)
        {
            Shop shop = GetShop(shopId);

            return shop?.Catalog.Generate();
        }

        private static long GetCost(long value, int discountUpper)
        {
            discountUpper = Math.Clamp(discountUpper, 0, 100);

            if (discountUpper == 0)
                return value;

            float discount = RangeF.Convert(0, 100, 0, 1, discountUpper);
            return (long) MathF.Floor(value * (1 - discount));
        }

        private static string WriteShopRow(Shop shop)
        {
            var row = new StringBuilder();

            row.AppendLine($"\n> `{shop.Id}` • **{shop.Name}**");

            if (!string.IsNullOrWhiteSpace(shop.Quote))
                row.AppendLine($"> {shop.Quote}");

            return row.ToString();
        }

        // Make sure to incorporate pagination
        public static string ViewShops()
        {
            // Incorporate the user into this
            // As it determines what shops they can currently view

            var info = new StringBuilder();

            info.AppendLine($"> 🛒 **Shops**");
            info.AppendLine($"> Here are all of the available shops.");


            foreach (Shop shop in Shops)
            {
                info.AppendLine(WriteShopRow(shop));
            }

            return info.ToString();
        }

        public static long CostOf(Item item, ItemCatalog catalog)
        {
            long value = item.Value;

            if (catalog.Discounts.ContainsKey(item.Id))
                return GetCost(value, catalog.Discounts[item.Id]);

            return value;
        }

        // This writes the catalog info
        public static string WriteCatalog(ItemCatalog catalog)
        {
            var info = new StringBuilder();

            foreach ((string itemId, int amount) in catalog.ItemIds)
            {
                int discountUpper = catalog.Discounts.ContainsKey(itemId) ? catalog.Discounts[itemId] : 0;
                info.AppendLine(WriteCatalogEntry(ItemHelper.GetItem(itemId), amount, discountUpper));
            }

            return info.ToString();
        }

        public static string WriteItemCost(Item item)
        {
            if (item.Value == 0)
                return "**Unknown Cost**";

            return $"{Icons.IconOf(item.Currency)} **{GetCost(item.Value, 0)}**";
        }

        public static string WriteDiscountRange(Item item, Shop shop)
        {
            if (item == null || shop == null)
                return "";

            var info = new StringBuilder();

            CatalogEntry entry = shop.Catalog?.Entries.FirstOrDefault(x => x.ItemId == item.Id);

            if (entry == null)
                return "";

            if (entry.DiscountChance <= 0)
                return "";

            info.Append($"(**{entry.MinDiscount ?? CatalogEntry.DefaultMinDiscount}**% to ");
            info.Append($"**{entry.MaxDiscount ?? CatalogEntry.DefaultMaxDiscount}**% discount range");

            info.Append($", **{RangeF.Convert(0, 1, 0, 100, entry.DiscountChance):##,0.##}**% chance of discount)");

            return info.ToString();
        }

        public static string WriteDeductionRange(Item item, Shop shop)
        {
            if (item == null || shop == null || shop.SellDeduction <= 0)
                return "";

            var info = new StringBuilder();

            info.Append($"(**{shop.SellDeduction}**% base sell deduction)");

            return info.ToString();
        }

        public static string WriteItemValue(Item item, int discount, ShopMode mode, bool showDetails = false)
        {
            var cost = new StringBuilder();
            cost.Append($"{Icons.IconOf(item.Currency)} ");

            if (item.Value == 0)
            {
                cost.Append("**Unknown Cost**");
                return cost.ToString();
            }

            discount = Math.Clamp(discount, 0, 100);

            if (discount == 100)
            {
                cost.Append(mode == ShopMode.Buy ? "**Free**" : "**Worthless**");
                return cost.ToString();
            }

            cost.Append($" **{GetCost(item.Value, discount):##,0}**");

            if (showDetails)
            {
                cost.Append($" (**{discount}**% ");
                cost.Append($"{(mode == ShopMode.Buy ? "discount" : "deduction")})");
            }

            return cost.ToString();
        }

        private static string WriteCatalogEntry(Item item, int amount, int discountUpper = 0)
        {
            var entry = new StringBuilder();

            // (isSpecial)
            //    entry.AppendLine("> 🍀 This item is special.");

            entry.Append($"\n> `{item.Id}` ");

            string icon = item.GetIcon();

            if (!string.IsNullOrWhiteSpace(icon))
                entry.Append($"{icon} ");

            entry.Append($"**{item.GetName()}**");

            if (amount > 1)
                entry.Append($" (x**{amount:##,0}**)");

            entry.AppendLine();

            if (Check.NotNullOrEmpty(item.Quotes))
                entry.AppendLine($"> *\"{item.GetQuote()}\"*");

            entry.Append($"> {WriteItemValue(item, discountUpper, ShopMode.Buy, true)} • *{item.Rarity}*");
            return entry.ToString();
        }

        public static string NameOf(string shopId)
            => GetShop(shopId).Name;
    }
}
