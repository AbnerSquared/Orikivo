using System.Text;
using Orikivo;

namespace Arcadia
{
    public class Inventory
    {
        private static string GetHeader(long capacity)
        {
            return $"> **Inventory**\n> `{GetCapacity(capacity)}` **{GetSuffix(capacity)}** available.";
        }

        private static string GetCapacity(long capacity)
        {
            var suffix = GetSuffix(capacity);

            return suffix switch
            {
                "B" => $"{capacity}",
                "KB" => $"{(double)(capacity / 1000)}",
                "MB" => $"{(double)(capacity / 1000000)}",
                "GB" => $"{(double)(capacity / 1000000000)}",
                "TB" => $"{(double)(capacity / 1000000000000)}",
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

        private static string WriteItem(int index, string id, ItemData data, bool isPrivate = true)
        {
            var item = ItemHelper.GetItem(id);
            var summary = new StringBuilder();

            summary.Append($"**#**{index}");

            if (!string.IsNullOrWhiteSpace(data.Data?.Id))
                summary.Append($" `{data.Data.Id}`");

            summary.AppendLine();
            summary.Append($"> `{id}` **{item.Name}**");
            
            if (data.Count > 1)
            {
                summary.Append($" (x**{data.Count}**)");
            }

            if (isPrivate) // Only write storage size if looking at your own inventory.
            {
                summary.AppendLine();
                summary.Append($"> `{GetCapacity(item.Size)}` **{GetSuffix(item.Size)}**");
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

        public static string Write(ArcadeUser user, bool isPrivate = true)
        {
            var inventory = new StringBuilder();

            if (isPrivate)
                inventory.AppendLine(GetHeader(user.GetStat(Vars.Capacity)));
            else
                inventory.AppendLine($"> **{user.Username}'s Inventory**");


            int i = 0;
            foreach (ItemData data in user.Items)
            {
                if (i > 0)
                {
                    inventory.AppendLine("\n");
                }

                inventory.AppendLine(WriteItem(i, data.Id, data, isPrivate));
                i++;
            }

            if (i == 0)
            {
                if (isPrivate)
                    inventory.AppendLine("> *\"I could not locate any files.\"*");
                else
                    inventory.AppendLine("> *\"This account does not have any items available for trade.\"*");
            }

            return inventory.ToString();
        }
    }
}