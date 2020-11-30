using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;

namespace Arcadia
{
    public static class CatalogHelper
    {
        public static string GetCatalogId(string itemId)
            => $"catalog:{itemId}";

        public static IEnumerable<Item> Search(string input)
            => Assets.Items.Where(x => MatchesAny(x, input));

        private static bool MatchesAny(Item item, string input)
        {
            if (item.Id == input)
                return true;

            ItemGroup group = ItemHelper.GetGroup(item.GroupId);

            if (group?.Id == input || (group?.Icon?.Equals(input) ?? false))
                return true;

            if (item.Rarity.ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
                return true;

            if (item.Tags.GetFlags().Any(x => x.ToString().Equals(input, StringComparison.OrdinalIgnoreCase)))
                return true;

            //if (Enum.TryParse(input, true, out ItemFilter filter))
            //    return MeetsFilter(item, filter);

            return item.Name.Contains(input, StringComparison.OrdinalIgnoreCase)
                   || (group?.Name?.Contains(input, StringComparison.OrdinalIgnoreCase)
                       ?? group?.Prefix?.Contains(input, StringComparison.OrdinalIgnoreCase)
                       ?? false);
        }

        public static bool MeetsFilter(string itemId, ItemFilter filter)
            => MeetsFilter(ItemHelper.GetItem(itemId), filter);

        public static bool MeetsFilter(Item item, ItemFilter filter)
        {
            return filter switch
            {
                ItemFilter.Ingredient => ItemHelper.IsIngredient(item),
                ItemFilter.Craftable => CraftHelper.CanCraft(item),
                ItemFilter.Sellable => ItemHelper.CanSell(item),
                ItemFilter.Buyable => ItemHelper.CanBuy(item),
                ItemFilter.Usable => item.Usage != null,
                ItemFilter.Tradable => ItemHelper.CanTrade(item),
                ItemFilter.Unique => ItemHelper.IsUnique(item),
                _ => false
            };
        }

        public static string GetFilterIcon(ItemFilter filter)
        {
            return filter switch
            {
                ItemFilter.Ingredient => "🧂",
                ItemFilter.Craftable => "📒",
                ItemFilter.Sellable => "📦",
                ItemFilter.Buyable => "🛍️",
                ItemFilter.Usable => "🔹",
                ItemFilter.Tradable => "📪",
                ItemFilter.Unique => "🔸",
                _ => null
            };
        }

        public static IEnumerable<Item> GetSeenItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && x.Value == (long)CatalogStatus.Seen)
                .Select(x => ItemHelper.GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetSeenItems(ArcadeUser user, string itemGroupId)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && ItemHelper.GroupOf(Var.GetKey(x.Key)) == itemGroupId
                            && x.Value == (long)CatalogStatus.Seen)
                .Select(x => ItemHelper.GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetKnownItems(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && x.Value == (long)CatalogStatus.Known)
                .Select(x => ItemHelper.GetItem(Var.GetKey(x.Key)));
        }

        public static IEnumerable<Item> GetKnownItems(ArcadeUser user, string itemGroupId)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && ItemHelper.GroupOf(Var.GetKey(x.Key)) == itemGroupId
                            && x.Value == (long)CatalogStatus.Known)
                .Select(x => ItemHelper.GetItem(Var.GetKey(x.Key)));
        }

        public static bool CanViewCatalog(ArcadeUser user)
        {
            return user.Stats.Any(x =>
                x.Key.StartsWith("catalog", StringComparison.OrdinalIgnoreCase)
                && (x.Value == (long)CatalogStatus.Seen || x.Value == (long)CatalogStatus.Known));
        }

        public static int GetKnownCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && x.Value == (long)CatalogStatus.Known);
        }

        public static int GetKnownCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && ItemHelper.GroupOf(Var.GetKey(x.Key)) == itemGroup
                            && x.Value == (long)CatalogStatus.Known);
        }

        public static int GetKnownCount(ArcadeUser user, ItemFilter filter)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && MeetsFilter(Var.GetKey(x.Key), filter)
                            && x.Value == (long)CatalogStatus.Known);
        }

        public static int GetVisibleCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && x.Value >= (long)CatalogStatus.Seen);
        }

        public static int GetVisibleCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x =>
                    x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                    && ItemHelper.GroupOf(Var.GetKey(x.Key)) == itemGroup
                    && x.Value >= (long)CatalogStatus.Seen);
        }

        public static int GetVisibleCount(ArcadeUser user, ItemFilter filter)
        {
            return user.Stats
                .Count(x =>
                    x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                    && MeetsFilter(Var.GetKey(x.Key), filter)
                    && x.Value >= (long)CatalogStatus.Seen);
        }

        public static int GetSeenCount(ArcadeUser user)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && x.Value == (long)CatalogStatus.Seen);
        }

        public static int GetSeenCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && ItemHelper.GroupOf(Var.GetKey(x.Key)) == itemGroup
                            && x.Value == (long)CatalogStatus.Seen);
        }

        public static int GetSeenCount(ArcadeUser user, ItemFilter filter)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && MeetsFilter(Var.GetKey(x.Key), filter)
                            && x.Value == (long)CatalogStatus.Seen);
        }



        public static CatalogStatus GetCatalogStatus(ArcadeUser user, string itemId)
        {
            if (ItemHelper.GroupOf(itemId) == Ids.Groups.Internal)
                return CatalogStatus.Unknown;

            long raw = user.GetVar(GetCatalogId(itemId));

            if (raw > (int)CatalogStatus.Known)
                return CatalogStatus.Known;

            return (CatalogStatus)raw;
        }

        public static CatalogStatus GetCatalogStatus(ArcadeUser user, Item item)
            => GetCatalogStatus(user, item.Id);

        public static void SetCatalogStatus(ArcadeUser user, string itemId, CatalogStatus status)
        {
            if (ItemHelper.GroupOf(itemId) == Ids.Groups.Internal)
                return;

            // If the user has already seen or known about this item, return
            if (GetCatalogStatus(user, itemId) >= status)
                return;

            user.SetVar(GetCatalogId(itemId), (long)status);
        }

        public static void SetCatalogStatus(ArcadeUser user, Item item, CatalogStatus status)
            => SetCatalogStatus(user, item.Id, status);

        public static bool HasAttributes(Item item)
        {
            return item.CanBuy
                   || item.CanSell
                   || item.BypassCriteriaOnTrade
                   || item.OwnLimit.HasValue
                   || (item.TradeLimit > 0 || !item.TradeLimit.HasValue)
                   || ItemHelper.IsUnique(item)
                   || HasUsageAttributes(item);
        }

        public static bool HasUsageAttributes(Item item)
        {
            if (item.Usage == null)
                return false;

            return item.Usage.Durability.HasValue
                   || item.Usage.Cooldown.HasValue
                   || item.Usage.Expiry.HasValue;
        }
    }
}