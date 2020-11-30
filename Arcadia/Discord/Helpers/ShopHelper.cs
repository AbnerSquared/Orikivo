﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia;
using Orikivo;
using Orikivo.Drawing;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public static class ShopHelper
    {
        internal static readonly int GroupSize = 5;

        // TODO: Move these over to the Var class, from which they can be set up as templates instead.
        // *:items_sold

        internal static string GetTotalSoldId(string shopId)
            => $"{shopId}:items_sold";

        internal static string GetVisitId(string shopId)
            => $"{shopId}:total_unique_visits";

        internal static string GetTotalBoughtId(string shopId)
            => $"{shopId}:items_bought";

        internal static string GetTotalSpentId(string shopId)
            => $"{shopId}:total_spent";

        internal static string GetTierId(string shopId)
            => $"{shopId}:tier";

        public static bool Exists(string shopId)
            => Assets.Shops.Any(x => x.Id == shopId);

        // TODO: Create ExistsFor methods in other helper classes (such as items, merits, etc.)
        // This is shop that the type readers can falsely let the user know that this merit does not exist, solely for them
        public static bool ExistsFor(ArcadeUser user, string shopId)
            => GetKnownShops(user).Any(x => x.Id == shopId);

        public static IEnumerable<Shop> GetKnownShops(ArcadeUser user)
        {
            // get the variable if not empty; otherwise, attempt to set it to 1 if they can visit
            return Assets.Shops.Where(x => Var.GetOrSet(user, GetTierId(x.Id), (x.ToVisit?.Invoke(user) ?? true) ? 1 : 0) > 0);
        }

        public static CatalogHistory HistoryOf(ArcadeUser user, string shopId)
        {
            if (!Exists(shopId))
                throw new Exception("The specified shop does not exist");

            if (!user.CatalogHistory.ContainsKey(shopId))
                user.CatalogHistory[shopId] = new CatalogHistory();

            return user.CatalogHistory[shopId];
        }

        public static IEnumerable<Vendor> GetVendors(List<string> catalogGroups)
        {
            return Assets.Vendors.Where(x => x.PreferredGroups.Any(catalogGroups.Contains));
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

            return shop.AllowedSellGroups.Contains(ItemHelper.GroupOf(data.Id));
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
            return (long)MathF.Floor(value * (1 - discount));
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
        public static string ViewShops(ArcadeUser user)
        {
            // Incorporate the user into this
            // As it determines what shops they can currently view

            var info = new StringBuilder();

            info.AppendLine($"> 🛒 **Shops**");

            TimeSpan remainder = TimeSpan.FromDays(1) - DateTime.UtcNow.TimeOfDay;
            info.AppendLine($"> All shops will restock in: **{Format.Countdown(remainder)}**");

            foreach (Shop shop in GetKnownShops(user))
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
        public static string WriteCatalog(CatalogGenerator generator, ItemCatalog catalog, int page = 0)
        {
            page = Paginate.ClampIndex(page, Paginate.GetPageCount(catalog.Count, GroupSize));
            Paginate.GroupAt(catalog.ItemIds, page, 5);

            var info = new StringBuilder();

            foreach ((string itemId, int amount) in Paginate.GroupAt(catalog.ItemIds, page, GroupSize))//catalog.ItemIds)
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

        public static string GetGenericReply(ShopState state)
        {
            return state switch
            {
                ShopState.Enter => Vendor.EnterGeneric,
                ShopState.Buy => Vendor.BuyGeneric,
                ShopState.BuyDeny => Vendor.BuyDenyGeneric,
                ShopState.BuyEmpty => Vendor.BuyEmptyGeneric,
                ShopState.BuyFail => Vendor.BuyFailGeneric,
                ShopState.BuyLimit => Vendor.BuyLimitGeneric,
                ShopState.SellNotAllowed => Vendor.SellNotAllowedGeneric,
                ShopState.SellNotOwned => Vendor.SellNotOwnedGeneric,
                ShopState.SellInvalid => Vendor.SellInvalidGeneric,
                ShopState.BuyInvalid => Vendor.BuyInvalidGeneric,
                ShopState.ViewBuy => Vendor.ViewBuyGeneric,
                ShopState.Sell => Vendor.SellGeneric,
                ShopState.ViewSell => Vendor.ViewSellGeneric,
                ShopState.SellDeny => Vendor.SellDenyGeneric,
                ShopState.SellEmpty => Vendor.SellEmptyGeneric,
                ShopState.Exit => Vendor.ExitGeneric,
                ShopState.Timeout => Vendor.TimeoutGeneric,
                ShopState.Menu => Vendor.MenuGeneric,
                ShopState.BuyRemainder => Vendor.BuyRemainderGeneric,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }

        public static string GetVendorReply(Vendor vendor, ShopState state)
        {
            var replies = state switch
            {
                ShopState.Enter => vendor?.OnEnter,
                ShopState.Buy => vendor?.OnBuy,
                ShopState.ViewBuy => vendor?.OnViewBuy,
                ShopState.BuyDeny => vendor?.OnBuyDeny,
                ShopState.BuyEmpty => vendor?.OnBuyEmpty,
                ShopState.BuyFail => vendor?.OnBuyFail,
                ShopState.BuyLimit => vendor?.OnBuyLimit,
                ShopState.SellNotAllowed => vendor?.OnSellNotAllowed,
                ShopState.SellNotOwned => vendor?.OnSellNotOwned,
                ShopState.SellInvalid => vendor?.OnSellInvalid,
                ShopState.BuyInvalid => vendor?.OnBuyInvalid,
                ShopState.Sell => vendor?.OnSell,
                ShopState.ViewSell => vendor?.OnViewSell,
                ShopState.SellDeny => vendor?.OnSellDeny,
                ShopState.SellEmpty => vendor?.OnSellEmpty,
                ShopState.Exit => vendor?.OnExit,
                ShopState.Timeout => vendor?.OnTimeout,
                ShopState.Menu => vendor?.OnMenu,
                ShopState.BuyRemainder => vendor?.OnBuyRemainder,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

            return Check.NotNullOrEmpty(replies) ? Randomizer.Choose(replies) : GetGenericReply(state);
        }
    }
}
