using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public class InventoryViewer
    {
        private static readonly int _rowSize = 10;
        private static readonly int _counterRowSize = 4;

        private static string GetHeader(long capacity, int pageCount = 1, int page = 0, string username = null)
        {
            page = Paginate.ClampIndex(page, pageCount);
            string extra = Format.PageCount(page, pageCount, " ({0})", false);
            var header = new StringBuilder(Locale.GetHeaderTitle(Headers.Inventory, extra, username));

            if (username == null)
                header.Append($"\n> You have {WriteCapacity(capacity)} available.\n");

            return header.ToString();
        }

        public static string WriteCapacity(long capacity)
        {
            StorageSize suffix = GetSuffix(capacity);
            return $"**{GetCapacity(capacity)} {((int)suffix >= 5 ? "" : suffix.ToString())}**";
        }

        private static string GetCapacity(long capacity)
        {
            StorageSize suffix = GetSuffix(capacity);
            double dividend = (double)1000 * (int)suffix;

            if (suffix == 0)
                dividend = 1;

            return suffix switch
            {
                _ when suffix >= StorageSize.Infinity => "∞",
                _ when suffix >= StorageSize.B => $"{capacity / dividend}",
                _ => throw new ArgumentOutOfRangeException(nameof(suffix), "The specified suffix is out of range")
            };
        }

        private static StorageSize GetSuffix(long capacity)
        {
            int len = capacity.ToString().Length;
            int count = (int)Math.Floor((len - 1) / (double)3);

            if (count > 5)
                return StorageSize.Infinity;

            return (StorageSize)count;
        }

        // Make this method generic for all viewers
        private static string WriteItemRow(string itemId, ItemData data)
        {
            Item item = ItemHelper.GetItem(itemId);

            string visibleId = data.Seal != null ? data.TempId : itemId;
            string icon = item.GetIcon();
            string name = Check.NotNull(icon) ? item.Name : item.GetName();

            var summary = new StringBuilder();

            summary.Append($"> ");

            summary.Append($"`{visibleId}`");

            if (!string.IsNullOrWhiteSpace(data.Data?.Id))
                summary.Append($"/`{data.Data.Id}`");

            summary.Append($" {(Check.NotNull(icon) ? icon : "•")} **{name}**");

            if (data.Count > 1)
            {
                summary.Append($" (x**{data.Count}**)");
            }

            return summary.ToString();
        }

        // NOTE: > **Slot {slot}:** `{id}`/`{unique_id}` {icon} **{name}** [{size}] (x**{count}**)
        private static string DrawRowText(bool exists, bool showSize, string visibleId, string uniqueId, string icon, string name, long size, int count, int? index = null)
        {
            var slot = new StringBuilder();

            slot.Append("> ");

            if (index.HasValue)
            {
                slot.Append("**Slot {index}**: ");
            }

            slot.Append($"`{visibleId}`");

            if (exists && !string.IsNullOrWhiteSpace(uniqueId))
                slot.Append($"/`{uniqueId}`");

            slot.Append($" {icon} **{name}**");

            if (exists)
            {
                if (showSize)
                    slot.Append($" [{WriteCapacity(size)}]");

                if (count > 1)
                    slot.Append($" (x**{count:##,0}**)");
            }

            return slot.ToString();
        }

        // > **Slot {slot}:** `{id}`/`{unique_id}` {icon ?? •} **{name}** [{size}] (x**{count}**)
        private static string BuildRowText(int index, string id, ItemData data, bool isPrivate = true)
        {
            bool exists = ItemHelper.Exists(id);

            if (!exists && !isPrivate)
            {
                throw new ArgumentException("Cannot view an external inventory item if the item does not exist");
            }

            Item item = ItemHelper.GetItem(id) ?? ItemHelper.GetItem(Ids.Items.InternalUnknown);

            string visibleId = !exists || data.Seal != null ? data.TempId : id;
            string icon = ItemHelper.IconOf(data);

            if (!Check.NotNull(icon))
                icon = "•";

            string name = ItemHelper.NameOf(data);

            return DrawRowText(exists, isPrivate, visibleId, data.Data?.Id, icon, name, item.Size, data.Count, index);
        }

        public static string ViewShopSellables(ArcadeUser user, Shop shop)
        {
            var result = new StringBuilder();

            string extra = null;

            if (shop.SellDeduction > 0)
            {
                extra = $"(**{shop.SellDeduction}**% deduction)";
            }

            result.AppendLine(Locale.GetHeaderTitle(Headers.Inventory, extra));

            foreach (ItemData data in user.Items.Where(x => ItemHelper.Exists(x.Id) && x.Seal == null && (ItemHelper.GetTag(x.Id) & shop.SellTags) != 0))
            {
                result.AppendLine(WriteItemRow(data.Id, data));
            }

            return result.ToString();
        }

        public static long GetReservedSize(ArcadeUser user)
        {
            return user.Items.Sum(x => ItemHelper.SizeOf(x.Id) * x.Count);
        }

        private static string DrawGroupCounters(IReadOnlyCollection<ItemData> items, int maxPerRow)
        {
            int i = 0;

            // TODO: Clean this horrible mess up lol
            IEnumerable<string> counters = items
                .Where(x =>
                    ItemHelper.Exists(x.Id)
                    && x.Seal != null
                    && ItemHelper.GroupOf(x.Id) != null
                    && !string.IsNullOrWhiteSpace(ItemHelper.GetGroupIcon(ItemHelper.GroupOf(x.Id))))
                .Select(x => $"{ItemHelper.GetGroup(x.Id).Icon} **x{items.Count(y => ItemHelper.Exists(y.Id) && y.Seal == null && ItemHelper.GroupOf(y.Id) == ItemHelper.GetGroup(x.Id).Id):##,0}**");

            var result = new StringBuilder();

            foreach (string counter in counters)
            {
                result
                    .Append(i % maxPerRow == 0 ? "\n> " : " ")
                    .Append(counter);

                i++;
            }

            return result.ToString();
        }

        public static string View(ArcadeUser user, bool isPrivate = true, int page = 0)
        {
            // set the default capacity if unspecified
            Var.SetIfEmpty(user, Vars.Capacity, 4000);
            var result = new StringBuilder();

            List<ItemData> items = isPrivate ? user.Items : user.Items.Where(x => x.Seal == null && ItemHelper.Exists(x.Id)).ToList();

            int pageCount = Paginate.GetPageCount(items.Count, _rowSize);
            result.AppendLine(GetHeader(user.GetVar(Vars.Capacity) - GetReservedSize(user), pageCount, page, isPrivate ? user.Username : null));

            if (items.Count == 0)
            {
                string onEmpty = isPrivate ? "Your inventory is empty." : "This inventory does not contain any visible items.";
                result.Append($"> *{onEmpty}*");

                return result.ToString();
            }

            if (pageCount > 1)
            {
                result.Append(DrawGroupCounters(items, _counterRowSize));
            }

            Paginate.GroupAt(items, page, _rowSize)
                .ForEach((x, i) => result.AppendLine(BuildRowText(i + 1, x.Id, x, isPrivate)));

            return result.ToString();
        }
    }
}
