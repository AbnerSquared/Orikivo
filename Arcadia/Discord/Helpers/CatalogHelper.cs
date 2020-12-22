using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public class Order
    {
        public string Id { get; set; }

        public int Position => CatalogHelper.GetOrderPosition(Id);

        public Item Item { get; set; }

        public DateTime DeliverAt { get; set; }
    }

    public static class CatalogHelper
    {
        public static string GetCatalogId(string itemId)
            => $"catalog:{itemId}";

        public static bool CanAfford(ArcadeUser user, long cost, CurrencyType currency)
        {
            long balance = CurrencyHelper.GetBalance(user, currency);

            return balance >= cost;
        }

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

        private static string GetOrderId(string itemId, int position)
            => $"order:{itemId}#{position}";

        public static string ViewOrders(ArcadeUser user, int page = 0)
        {
            var result = new TextBody();

            int orderCount = GetOrderCount(user);

            result.Header = new Header
            {
                Title = "Orders",
                Icon = "📦",
                Subtitle = $"Active Orders: **{orderCount:##,0}**"
            };

            // result.Tooltips.Add("Type `ordercancel <position>` to cancel an active order.");

            result.Sections.Add(GetOrderSection(user, page));

            return result.Build(user.Config.Tooltips);
        }

        private static readonly int OrderSectionSize = 5;

        private static TextSection GetOrderSection(ArcadeUser user, int page = 0)
        {
            var content = new StringBuilder();

            page = Paginate.ClampIndex(page, Paginate.GetPageCount(GetOrderCount(user), OrderSectionSize));

            foreach (Order order in Paginate.GroupAt(GetOrders(user), page, OrderSectionSize))
            {
                content.AppendLine(ViewOrder(order)).AppendLine();
            }

            return new TextSection
            {
                Content = content.ToString()
            };
        }

        private static string ViewOrder(Order order)
        {
            var result = new StringBuilder();

            // #**{order.Position + 1}**
            result.AppendLine($"> {ViewItemName(order.Item)}");
            result.Append($"> Arrives in **{Format.Countdown(order.DeliverAt - DateTime.UtcNow)}**");

            return result.ToString();
        }

        private static string ViewItemName(Item item)
        {
            return $"{item.GetIcon() ?? "•"} **{item.Name}**";
        }

        public static IEnumerable<Order> GetOrders(ArcadeUser user)
        {
            return user.Stats
                .Where(x => x.Key.StartsWith("order:", StringComparison.OrdinalIgnoreCase)
                    && ItemHelper.Exists(Var.GetKey(x.Key).Split('#')[0]))
                .Select(x => new Order
                {
                    Id = x.Key,
                    Item = ItemHelper.GetItem(Var.GetKey(x.Key).Split('#')[0]),
                    DeliverAt = new DateTime(x.Value)
                })
                .OrderBy(x => x.DeliverAt);
        }

        public static int GetOrderCount(ArcadeUser user)
        => user.Stats.Count(x => x.Key.StartsWith("order:", StringComparison.OrdinalIgnoreCase)
                    && ItemHelper.Exists(Var.GetKey(x.Key).Split('#')[0]));

        public static void TryCompleteOrders(ArcadeUser user)
        {
            bool updateOrders = false;
            int completedOrders = 0;

            foreach (Order order in GetOrders(user))
            {
                bool isComplete = TryCompleteOrder(user, order, false);

                updateOrders |= isComplete;

                if (isComplete)
                    completedOrders++;
            }

            if (completedOrders > 0 && user.Config.Notifier.HasFlag(NotifyAllow.ItemInbound))
            {
                if (completedOrders == 1)
                    user.Notifier.Add("An order has been delivered!");
                else
                {
                    user.Notifier.Add($"{completedOrders:##,0} orders have been delivered!");
                }
            }

            if (updateOrders && GetOrderCount(user) > 0)
                UpdateOrderPositions(user);
        }

        private static bool TryCompleteOrder(ArcadeUser user, Order order, bool notify = true)
        {
            if (!CooldownHelper.IsExpired(order.DeliverAt))
                return false;

            Var.Clear(user, order.Id); // Remove this order from the list
            ItemData package = ItemHelper.CreateData(order.Item, 1, Ids.Items.InternalPackage);
            ItemHelper.AddItem(user, package);

            if (notify && user.Config.Notifier.HasFlag(NotifyAllow.ItemInbound))
                user.Notifier.Add("An order has been delivered!");

            return true;

        }

        internal static double GetOrderHours(ItemRarity rarity)
            => (int)rarity * 12;

        internal static int GetOrderPosition(string orderId)
        {
            if (!Check.NotNull(orderId))
                throw new ArgumentException("Expected a non-empty string instance");

            var reader = new StringReader(orderId);

            reader.SkipUntil('#', true);

            return int.Parse(reader.GetRemaining());
        }

        private static void UpdateOrderPositions(ArcadeUser user)
        {
            int i = 0;
            int offset = 0;

            foreach ((string id, _) in user.Stats.Where(x => x.Key.StartsWith("order:", StringComparison.OrdinalIgnoreCase)
                    && ItemHelper.Exists(Var.GetKey(x.Key).Split('#')[0])).OrderBy(y => GetOrderPosition(y.Key)))
            {
                int position = GetOrderPosition(id);

                if (i == 0)
                    offset = position;

                if (offset == 0)
                    return;

                string newId = $"{id.Split('#')[0]}#{position - offset}";
                Var.Rename(user, id, newId);

                i++;
            }
        }

        public static bool TryAddOrder(ArcadeUser user, Item item)
        {
            // Attempt to clear out any completed orders
            TryCompleteOrders(user);

            int activeOrderCount = GetOrderCount(user);

            // Prohibit new orders if there is already too many orders
            if (activeOrderCount >= Var.GetValue(user, Vars.OrderLimit))
                return false;

            long deliveryTime = DateTime.UtcNow.AddHours(GetOrderHours(item.Rarity)).Ticks;

            user.SetVar(GetOrderId(item.Id, GetOrderCount(user)), deliveryTime);
            user.AddToVar(Stats.Common.ItemsOrdered);
            return true;
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
                            && x.Value >= (long)CatalogStatus.Known);
        }

        public static int GetKnownCount(ArcadeUser user, IEnumerable<Item> entries)
        {
            return user.Stats.Where(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase) && entries.Any(y => y.Id == Var.GetKey(x.Key)))
                .Count(x => x.Value >= (long)CatalogStatus.Known);
        }

        public static int GetSeenCount(ArcadeUser user, IEnumerable<Item> entries)
        {
            return user.Stats.Where(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase) && entries.Any(y => y.Id == Var.GetKey(x.Key)))
                .Count(x => x.Value == (long)CatalogStatus.Seen);
        }

        public static int GetKnownCount(ArcadeUser user, string itemGroup)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && ItemHelper.GroupOf(Var.GetKey(x.Key)) == itemGroup
                            && x.Value >= (long)CatalogStatus.Known);
        }

        public static int GetKnownCount(ArcadeUser user, ItemFilter filter)
        {
            return user.Stats
                .Count(x => x.Key.StartsWith("catalog:", StringComparison.OrdinalIgnoreCase)
                            && MeetsFilter(Var.GetKey(x.Key), filter)
                            && x.Value >= (long)CatalogStatus.Known);
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
                   || item.Usage.Timer.HasValue;
        }
    }
}