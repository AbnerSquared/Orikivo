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
        // TODO: Add pagination and group counter icon support
        private static string GetHeader(long capacity, int pageCount = 1, int page = 0)
        {
            page = page < 0 ? 0 : page > pageCount - 1 ? pageCount - 1 : page;
            string extra = pageCount > 1 ? $" (Page **{page:##,0}** of **{pageCount:##,0}**)" : "";
            var header = new StringBuilder(Locale.GetHeaderTitle(Headers.Inventory, extra));

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
            double dividend = ((double)1000 * ((int)suffix));

            if ((int)suffix == 0)
                dividend = 1;

            return suffix switch
            {
                _ when suffix >= StorageSize.Infinity => "∞",
                _ when suffix >= StorageSize.B  => $"{capacity / dividend}",
                _ => throw new ArgumentOutOfRangeException(nameof(suffix), "The specified suffix is out of range")
            };
        }

        private static StorageSize GetSuffix(long capacity)
        {
            int len = capacity.ToString().Length;

            //if (len - 1 <= 3)
            //    return StorageSize.B;

            int count = (int) Math.Floor((len - 1) / (double) 3);


            // 4 7 10 13 16
            // 1 3  7 10 13
            // 3 6  9 12 15
            if (count > 5)
                return StorageSize.Infinity;

            return (StorageSize)(count);
        }

        private static string WriteItemRow(int index, string id, ItemData data, int deduction)
        {
            Item item = ItemHelper.GetItem(id);
            var summary = new StringBuilder();

            summary.Append($"> ");

            summary.Append($"`{id}`");

            if (!string.IsNullOrWhiteSpace(data.Data?.Id))
                summary.Append($"/`{data.Data.Id}`");

            summary.Append($" • **{item.GetName()}**");

            if (data.Count > 1)
            {
                summary.Append($" (x**{data.Count}**)");
            }

            return summary.ToString();
        }

        private static string WriteItemRow(int index, string id, ItemData data)
        {
            Item item = ItemHelper.GetItem(id);

            string visibleId = data.Seal != null ? data.TempId : id;
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

        // > **Slot {slot}:** `{id}`/`{unique_id}` {icon ?? •} **{name}** [{size}] (x**{count}**)
        private static string WriteItem(int index, string id, ItemData data, bool isPrivate = true, bool showTooltips = true)
        {
            bool exists = ItemHelper.Exists(id);
            if (!exists)
            {
                if (!isPrivate)
                {
                    throw new ArgumentException("Cannot view an external inventory item if the item does not exist");
                }
            }

            Item item = ItemHelper.GetItem(id) ?? ItemHelper.GetItem(Ids.Items.InternalUnknown);
            var summary = new StringBuilder();

            string visibleId = !exists || data.Seal != null ? data.TempId : id;
            string icon = data.Seal != null ? ItemHelper.IconOf(data.Seal.ReferenceId) : item.GetIcon();
            string name = (data.Seal != null ? ItemHelper.NameOf(data.Seal.ReferenceId) : (Check.NotNull(icon) ? item.Name : item.GetName())) ?? "Unknown Item";


            summary.Append($"> **Slot {index}:** ");
            summary.Append($"`{visibleId}`");

            if (exists)
            {
                if (!string.IsNullOrWhiteSpace(data.Data?.Id))
                    summary.Append($"/`{data.Data.Id}`");
            }

            summary.Append($" {(Check.NotNull(icon) ? icon : "•")} **{name}**");

            if (exists)
            {
                if (isPrivate && showTooltips)
                    summary.Append($" ({WriteCapacity(item.Size)})");

                if (data.Count > 1)
                    summary.Append($" (x**{data.Count}**)");
            }

            return summary.ToString();
        }

        public static string ViewShopSellables(ArcadeUser user, Shop shop)
        {
            var inventory = new StringBuilder();

            if (shop.SellDeduction > 0)
            {
                inventory.AppendLine(Locale.GetHeaderTitle(Headers.Inventory, $"(**{shop.SellDeduction}**% deduction)"));
            }

            int i = 0;
            foreach (ItemData data in user.Items.Where(x => x.Seal == null && ((ItemHelper.GetTag(x.Id) & shop.SellTags) != 0)))
            {
                inventory.AppendLine($"{WriteItemRow(i + 1, data.Id, data)}");
                i++;
            }

            return inventory.ToString();
        }

        public static long GetInventorySize(ArcadeUser user)
        {
            return user.Items.Sum(x => ItemHelper.SizeOf(x.Id) * x.Count);
        }

        public static string View(ArcadeUser user, bool isPrivate = true, int page = 0)
        {
            // set the default capacity if unspecified
            Var.SetIfEmpty(user, Vars.Capacity, 4000);
            var inventory = new StringBuilder();

            List<ItemData> items = isPrivate ? user.Items : user.Items.Where(x => x.Seal == null && ItemHelper.Exists(x.Id)).ToList();

            int pageCount = Paginate.GetPageCount(items.Count, _rowSize);

            if (isPrivate)
            {
                inventory.AppendLine(GetHeader(user.GetVar(Vars.Capacity) - GetInventorySize(user), pageCount, page));
            }
            else
            {
                page = page < 0 ? 0 : page > pageCount - 1 ? pageCount - 1 : page;
                string extra = pageCount > 1 ? $" (Page **{page:##,0}** of **{pageCount:##,0}**)" : null;
                inventory.AppendLine(Locale.GetHeaderTitle(Headers.Inventory, extra, user.Username))
                    .AppendLine();
            }

            if (pageCount > 1)
            {
                int c = 0;
                foreach (string counter in items
                    .Where(x => x.Seal == null && ItemHelper.GroupOf(x.Id) != null)
                    .Select(x => ItemHelper.GetGroup(ItemHelper.GroupOf(x.Id)))
                    .Where(x => x.Icon != null)
                    .Select(g => $"{g.Icon.ToString()} **x{items.Count(x => x.Seal == null && ItemHelper.GroupOf(x.Id) == g.Id):##,0}**"))
                {
                    if (c % 4 == 0)
                        inventory.Append("\n> ");
                    else
                        inventory.Append(" ");

                    inventory.Append(counter);
                    c++;
                }
            }

            int i = 0;

            foreach (ItemData data in Paginate.GroupAt(items, page, _rowSize))
            {
                inventory.AppendLine(WriteItem(i + 1, data.Id, data, isPrivate));
                i++;
            }

            if (i == 0)
            {
                if (isPrivate)
                    inventory.AppendLine("> *Your inventory is empty.*");
                else
                    inventory.AppendLine("> *This inventory does not contain any visible items.*");
            }

            return inventory.ToString();
        }
    }
}