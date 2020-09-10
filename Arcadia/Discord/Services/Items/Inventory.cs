﻿using System;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Desync;

namespace Arcadia
{
    public class Inventory
    {
        private static string GetHeader(long capacity, bool showTooltips = true)
        {
            var header = new StringBuilder(Locale.GetHeader(Headers.Inventory));

            if (showTooltips)
                header.Append($"\n> You have {WriteCapacity(capacity)} available.");

            header.Append("\n");

            return header.ToString();
        }

        public static string WriteCapacity(long capacity)
        {
            StorageSize suffix = GetSuffix(capacity);
            return $"**{GetCapacity(capacity)} {((int)suffix >= 5 ? "" : suffix.ToString())}**";
        }

        // TODO: The capacity determination could be cleaned up.
        private static string GetCapacity(long capacity)
        {
            StorageSize suffix = GetSuffix(capacity);

            return suffix switch
            {
                _ when (int) suffix >= 5 => "∞",
                _ when suffix >= 0 => $"{capacity / ((double) 1000 * (int) suffix)}",
                _ => throw new ArgumentOutOfRangeException(nameof(suffix), "The specified suffix is out of range")
            };
        }

        private static StorageSize GetSuffix(long capacity)
        {
            int len = capacity.ToString().Length;

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
            Item item = ItemHelper.GetItem(id);
            var summary = new StringBuilder();

            string visibleId = data.Seal != null ? data.TempId : id;
            string icon = data.Seal != null ? ItemHelper.IconOf(data.Seal.ReferenceId) : item.GetIcon();
            string name = (data.Seal != null ? ItemHelper.NameOf(data.Seal.ReferenceId) : (Check.NotNull(icon) ? item.Name : item.GetName())) ?? "Unknown Item";


            summary.Append($"> **Slot {index}:** ");

            summary.Append($"`{visibleId}`");

            if (!string.IsNullOrWhiteSpace(data.Data?.Id))
                summary.Append($"/`{data.Data.Id}`");

            summary.Append($" {(Check.NotNull(icon) ? icon : "•")} **{name}**");

            if (isPrivate && showTooltips)
                summary.Append($" ({WriteCapacity(item.Size)})");

            if (data.Count > 1)
                summary.Append($" (x**{data.Count}**)");

            // if (!isPrivate && !ItemHelper.CanTrade(item.Id, data.Data))
            //    summary.Append("\n> ⚠️ This item is untradable.");

            return summary.ToString();
        }

        public static string WriteItems(ArcadeUser user, Shop shop)
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

        private static long GetInventorySize(ArcadeUser user)
        {
            return user.Items.Sum(x => ItemHelper.SizeOf(x.Id) * x.Count);
        }

        public static string Write(ArcadeUser user, bool isPrivate = true)
        {
            // set the default capacity if unspecified
            Var.SetIfEmpty(user, Vars.Capacity, 4000);
            var inventory = new StringBuilder();

            if (isPrivate)
                inventory.AppendLine(GetHeader(user.GetVar(Vars.Capacity) - GetInventorySize(user)));
            else
                inventory.AppendLine($"{Locale.GetHeaderTitle(Headers.Inventory, group: user.Username)}\n");


            int i = 0;
            foreach (ItemData data in user.Items)
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