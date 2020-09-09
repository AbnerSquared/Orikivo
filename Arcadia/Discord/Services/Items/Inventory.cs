using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    public static class Profile
    {
        public static string View(ArcadeUser user, ArcadeContext ctx)
        {
            var details = new StringBuilder();

            details.AppendLine($"> **{user.Username}**");
            details.AppendLine($"> Joined: **{Format.Date(user.CreatedAt, '.')}**");

            if (user.Balance > 0 || user.Debt > 0 || user.ChipBalance > 0)
            {
                details.AppendLine("\n> **Wallet**");

                if (user.Balance > 0 || user.Debt > 0)
                {
                    string icon = user.Balance > 0 ? Icons.Balance : Icons.Debt;
                    long value = user.Balance > 0 ? user.Balance : user.Debt;
                    string id = user.Balance > 0 ? Vars.Balance : Vars.Debt;
                    int position = Leaderboard.GetPosition(ctx.Data.Users.Values.Values, user, id);
                    string pos = "";
                    if (position < 4)
                        pos = $" (#**{position:##,0}** global)";

                    details.AppendLine($"> {icon} **{value:##,0}**{pos}");
                }

                if (user.ChipBalance > 0)
                {
                    int position = Leaderboard.GetPosition(ctx.Data.Users.Values.Values, user, Vars.Chips);
                    string pos = "";
                    if (position < 4)
                        pos = $" (#**{position:##,0}** global)";
                    details.AppendLine($"> {Icons.Chips} **{user.ChipBalance:##,0}**{pos}");
                }

                if (user.Items.Count > 0)
                {
                    details.AppendLine($"\n> **Inventory**");
                    details.AppendLine($"> **{user.Items.Count}** used {Format.TryPluralize("slot", user.Items.Count)}");
                }

                var chosen = new List<string>();
                string stat = StatHelper.GetRandomStat(user, chosen);

                while (!string.IsNullOrWhiteSpace(stat))
                {
                    if (chosen.Count >= 3) // The amount of random stats to show
                        break;

                    if (chosen.Count == 0)
                        details.AppendLine($"\n> **Random Statistics**");

                    details.AppendLine($"> **{Var.WriteName(stat)}**: {Var.WriteValue(user, stat)}");

                    chosen.Add(stat);
                    stat = StatHelper.GetRandomStat(user, chosen);
                }
            }

            return details.ToString();
        }

    }
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
            return $"**{GetCapacity(capacity)} {GetSuffix(capacity)}**";
        }

        // TODO: The capacity determination could be cleaned up.
        private static string GetCapacity(long capacity)
        {
            string suffix = GetSuffix(capacity);

            return suffix switch
            {
                "B" => $"{capacity}",
                "KB" => $"{capacity / (double) 1000}",
                "MB" => $"{capacity / (double) 1000000}",
                "GB" => $"{capacity / (double) 1000000000}",
                "TB" => $"{capacity / (double) 1000000000000}",
                _ => "∞"
            };
        }

        private static string GetSuffix(long capacity)
        {
            int len = capacity.ToString().Length;

            if (len < 4)
                return "B";

            if (len < 7)
                return "KB";

            if (len < 10)
                return "MB";

            if (len < 13)
                return "GB";

            if (len < 16)
                return "TB";

            return "PB";
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