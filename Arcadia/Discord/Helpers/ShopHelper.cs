using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia
{
    public static class ShopHelper
    {
        internal static string GetTotalSoldId(string shopId)
            => $"{shopId}:items_sold";

        internal static string GetVisitId(string shopId)
            => $"{shopId}:total_unique_visits";

        internal static string GetTotalBoughtId(string shopId)
            => $"{shopId}:items_bought";

        internal static string GetTierId(string shopId)
            => $"{shopId}:tier";

        public static bool Exists(string shopId)
            => Assets.Shops.Any(x => x.Id == shopId);

        public static CatalogHistory HistoryOf(ArcadeUser user, string shopId)
        {
            if (!Exists(shopId))
                throw new Exception("The specified shop does not exist");

            if (!user.CatalogHistory.ContainsKey(shopId))
                user.CatalogHistory[shopId] = new CatalogHistory();

            return user.CatalogHistory[shopId];
        }

        public static IEnumerable<Vendor> GetVendors(ItemTag catalogTags)
        {
            return Assets.Vendors.Where(x => (x.PreferredTag & catalogTags) != 0);
        }

        // sum together all unique tags
        public static ItemTag GetUniqueTags(ItemCatalog catalog)
        {
            ItemTag unique = 0;

            foreach ((string itemId, int amount) in catalog.ItemIds)
                unique |= ItemHelper.GetTag(itemId);

            return unique;
        }

        public static bool CanSell(Shop shop, ItemData data)
        {
            Item item = ItemHelper.GetItem(data.Id);

            if (item == null)
                throw new Exception("Invalid data instance specified");

            return (item.Tag & shop.SellTags) != 0;
        }

        public static string Sell(Shop shop, ItemData data, ArcadeUser user)
        {
            if (!CanSell(shop, data))
            {
                return Format.Warning($"**{shop.Name}** does not accept this item.");
            }

            Item item = ItemHelper.GetItem(data.Id);
            ItemHelper.TakeItem(user, data);

            long value = shop.SellDeduction > 0
                ? (long)Math.Floor(item.Value * (1 - shop.SellDeduction / (double)100))
                : item.Value;

            user.Give(value, item.Currency);
            string icon = (Check.NotNull(item.GetIcon()) ? $"{item.GetIcon()} " : "");
            string name = $"{icon}**{(Check.NotNull(icon) ? item.Name : item.GetName())}**";

            return $"> You have received {Icons.IconOf(item.Currency)} **{value:##,0}** for {name}.";
        }

        public static Shop GetShop(string id)
        {
            if (Assets.Shops.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one shops with the specified ID.");

            return Assets.Shops.FirstOrDefault(x => x.Id == id);
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
        public static string ViewShops(ArcadeData data)
        {
            // Incorporate the user into this
            // As it determines what shops they can currently view

            var info = new StringBuilder();

            info.AppendLine($"> 🛒 **Shops**");

            TimeSpan remainder = TimeSpan.FromDays(1) - DateTime.UtcNow.TimeOfDay;
            info.AppendLine($"> All shops will restock in: **{Format.Countdown(remainder)}**");

            foreach (Shop shop in Assets.Shops)
                info.Append(WriteShopRow(shop));

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
        public static string WriteCatalog(CatalogGenerator generator, ItemCatalog catalog)
        {
            var info = new StringBuilder();

            foreach ((string itemId, int amount) in catalog.ItemIds)
            {
                int discountUpper = catalog.Discounts.ContainsKey(itemId) ? catalog.Discounts[itemId] : 0;
                info.AppendLine(WriteCatalogEntry(ItemHelper.GetItem(itemId), amount, generator.Entries.Any(x => x.ItemId == itemId && x.IsSpecial), discountUpper));
            }

            return info.ToString();
        }

        public static string WriteItemCost(Item item)
        {
            return CurrencyHelper.WriteCost(GetCost(item.Value, 0), item.Currency);
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

            if (showDetails && discount > 0)
            {
                cost.Append($" (**{discount}**% {(mode == ShopMode.Buy ? "discount" : "deduction")})");
            }

            return cost.ToString();
        }

        private static string WriteCatalogEntry(Item item, int amount, bool isSpecial, int discountUpper = 0)
        {
            var entry = new StringBuilder();

            entry.Append($"\n> {(isSpecial ? "🍀 " : "")}`{item.Id}` ");

            string icon = item.GetIcon();

            if (!string.IsNullOrWhiteSpace(icon))
                entry.Append($"{icon} ");

            entry.Append($"**{(!string.IsNullOrWhiteSpace(icon) ? item.Name : item.GetName())}** ({item.Rarity})");

            if (amount > 1)
                entry.Append($" (x**{amount:##,0}**)");

            entry.AppendLine();

            if (Check.NotNullOrEmpty(item.Quotes))
                entry.AppendLine($"> *\"{item.GetQuote()}\"*");

            entry.Append($"> {WriteItemValue(item, discountUpper, ShopMode.Buy, true)} • {InventoryViewer.WriteCapacity(item.Size)}");
            return entry.ToString();
        }

        public static string NameOf(string shopId)
            => GetShop(shopId).Name;
    }
}
