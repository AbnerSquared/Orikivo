﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public class InventoryViewer
    {
        private static readonly int StackGroupSize = 10;
        private static readonly int CounterGroupSize = 4;

        private static string GetHeader(int pageCount = 1, int page = 0, string username = null)
        {
            page = Paginate.ClampIndex(page, pageCount);
            string extra = Format.PageCount(page, pageCount, " ({0})", false);
            var header = new StringBuilder(Locale.GetHeaderTitle(Headers.Inventory, extra, username));

            return header.ToString();
        }

        public static string PreviewItemStack(ItemData data, bool isPrivate = true, int? index = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "The specified item stack cannot be null");
            }

            bool exists = ItemHelper.Exists(data.Id);

            if (!exists && !isPrivate)
            {
                throw new ArgumentException("Cannot preview a public item stack if the item does not exist");
            }

            Item item = ItemHelper.GetItem(data.Id) ?? ItemHelper.GetItem(Ids.Items.InternalUnknown);

            string visibleId = !exists || data.Seal != null ? data.TempId : data.Id;
            string icon = ItemHelper.GetIconOrDefault(data);

            if (!Check.NotNull(icon))
                icon = "•";

            string name = ItemHelper.NameOf(data);

            return DrawItemStack(exists, visibleId, data.Data?.Id, icon, name, data.Count, index);
        }

        public static long GetReservedSize(ArcadeUser user)
        {
            return user.Items.Sum(x => ItemHelper.SizeOf(x.Id) * x.Count);
        }

        public static string View(ArcadeUser user, bool isPrivate = true, int page = 0)
        {
            Var.SetIfEmpty(user, Vars.Capacity, 4000);
            var result = new StringBuilder();

            List<ItemData> items = isPrivate ? user.Items : user.Items.Where(x => x.Seal == null && ItemHelper.Exists(x.Id)).ToList();

            int pageCount = Paginate.GetPageCount(items.Count, StackGroupSize);
            result.AppendLine(GetHeader(pageCount, page, isPrivate ? null : user.Username));

            if (items.Count == 0)
            {
                string onEmpty = isPrivate ? "Your inventory is empty." : "This inventory does not have any available items for trade.";
                result.Append($"> *{onEmpty}*");

                return result.ToString();
            }

            if (pageCount > 1)
            {
                result.Append(GetGroupCounters(items, CounterGroupSize));
            }

            Paginate.GroupAt(items, page, StackGroupSize)
                .ForEach((x, i) => result.AppendLine(PreviewItemStack(x, isPrivate, i + 1)));

            return result.ToString();
        }

        private static string GetGroupCounters(IReadOnlyCollection<ItemData> items, int maxPerRow)
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

        private static string DrawItemStack(bool exists, string visibleId, string uniqueId, string icon, string name, int count, int? index = null)
        {
            var slot = new StringBuilder();

            slot.Append("> ");

            if (index.HasValue)
            {
                slot.Append($"**Slot {index}**: ");
            }

            slot.Append($"`{visibleId}`");

            if (exists && !string.IsNullOrWhiteSpace(uniqueId))
                slot.Append($"/`{uniqueId}`");

            slot.Append($" {icon} **{name}**");

            if (exists)
            {
                if (count > 1)
                    slot.Append($" (x**{count:##,0}**)");
            }

            return slot.ToString();
        }
    }
}
