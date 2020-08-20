using System.Text;
using Orikivo;

namespace Arcadia
{
    public class Inventory
    {
        private static string GetHeader(long capacity, bool showTooltips = true)
        {
            var header = new StringBuilder("> 📂 **Inventory**\n> View your contents currently in storage.");

            if (showTooltips)
                header.Append($"\n> You have **{GetCapacity(capacity)}**{GetSuffix(capacity)} available.");

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
            var summary = new StringBuilder();

            summary.Append($"> **Slot {index}:** ");

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

        private static string WriteItem(int index, string id, ItemData data, bool isPrivate = true, bool showTooltips = true)
        {
            Item item = ItemHelper.GetItem(id);
            var summary = new StringBuilder();

            summary.Append($"> **Slot {index}:** ");

            summary.Append($"`{id}`");

            if (!string.IsNullOrWhiteSpace(data.Data?.Id))
                summary.Append($"/`{data.Data.Id}`");

            summary.Append($" • **{item.GetName()}**");
            
            if (data.Count > 1)
            {
                summary.Append($" (x**{data.Count}**)");
            }

            if (isPrivate) // Only write storage size if looking at your own inventory.
            {
                if (showTooltips)
                {
                    summary.Append($" (**{GetCapacity(item.Size)}**{GetSuffix(item.Size)})");
                }
            }
            else
            {
                if (!ItemHelper.CanTrade(item.Id, data?.Data))
                {
                    summary.Append("\n> ⚠️ This item is untradable.");
                }
            }

            return summary.ToString();
        }

        public static string WriteItems(ArcadeUser user, Shop shop)
        {
            var inventory = new StringBuilder();

            if (shop.SellDeduction > 0)
            {
                inventory.AppendLine($"> **Inventory** (**{shop.SellDeduction}**% deduction)");
            }

            int i = 0;
            foreach (ItemData data in user.Items)
            {
                ItemTag tag = ItemHelper.GetTag(data.Id);

                // Ignore if this cannot be sold.
                if ((tag & shop.SellTags) == 0) // if both tags don't have any matching tags, ignore this entry
                    continue;

                inventory.AppendLine($"\n{WriteItemRow(i + 1, data.Id, data)}");
                i++;
            }

            return inventory.ToString();
        }

        public static string Write(ArcadeUser user, bool isPrivate = true)
        {
            // set the default capacity if unspecified
            StatHelper.SetIfEmpty(user, Vars.Capacity, 4000);
            var inventory = new StringBuilder();

            if (isPrivate)
                inventory.AppendLine(GetHeader(user.GetVar(Vars.Capacity)));
            else
                inventory.AppendLine($"> 📂 **{user.Username}'s Inventory**\n");


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
                    inventory.AppendLine("> *This inventory does not contain any tradable items.*");
            }

            return inventory.ToString();
        }
    }
}