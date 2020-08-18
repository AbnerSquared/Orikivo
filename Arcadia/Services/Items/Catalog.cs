using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia.Services
{
    public class Catalog
    {
        private static string _line = "> **{0}**: {1}";
        private static string GetId(Item item)
            => string.Format(_line, "ID", $"`{item.Id}`");

        private static string GetName(Item item)
            => string.Format(_line, "Name", $"**`{ItemHelper.NameOf(item.Id)}`**");

        private static string GetSummary(Item item)
            => string.Format(_line, "Summary", $"`{item.Summary}`");

        private static string GetQuotes(Item item)
            => string.Format(_line, Format.TryPluralize("Quote", item.Quotes.Count), string.Join(", ", item.Quotes.Select(x => $"*`\"{x}\"`*")));

        private static string GetRarity(Item item)
            => string.Format(_line, "Rarity", $"`{item.Rarity.ToString()}`");

        private static string GetTags(Item item)
            => string.Format(_line, Format.TryPluralize("Tag", item.Tag.GetActiveFlags().Count()), string.Join(", ", item.Tag.GetActiveFlags().Select(x => $"`{x.ToString()}`")));

        private static string GetValue(Item item)
            => string.Format(_line, "Value", $"**`{item.Value:##,0}`**");

        private static string GetBuyState(Item item)
            => string.Format(_line, "Can Buy?", item.CanBuy ? "`Yes`": "`No`");

        private static string GetSellState(Item item)
            => string.Format(_line, "Can Sell?", item.CanSell ? "`Yes`" : "`No`");

        private static string GetTradeState(Item item)
            => string.Format(_line, "Can Trade?", item.TradeLimit.HasValue ? item.TradeLimit.Value == 0 ? "`No`" : $"`Yes (x{item.TradeLimit.Value.ToString("##,0")})`" : "`Yes`");

        private static string GetGiftState(Item item)
            => string.Format(_line, "Can Gift?", item.GiftLimit.HasValue ? item.GiftLimit.Value == 0 ? "`No`" : $"`Yes (x{item.GiftLimit.Value.ToString("##,0")})`" : "`Yes`");

        private static string GetUseState(Item item)
            => string.Format(_line, "Can Use?", item.Usage != null ? "`Yes`": "`No`");

        private static string GetUniqueState(Item item)
            => string.Format(_line, "Is Unique?", ItemHelper.IsUnique(item) ? "`Yes`" : "`No`");

        private static string GetBypassState(Item item)
            => string.Format(_line, "Bypass Requirements On Gift?", item.BypassCriteriaOnGift ? "`Yes`" : "`No`");

        private static string GetOwnLimit(Item item)
            => string.Format(_line, "Own Limit", item.OwnLimit.HasValue ? $"`{item.OwnLimit.Value:##,0}`" : "`None`");

        // this is only the definer
        public static string WriteItem(Item item)
        {
            var details = new StringBuilder();

            details.AppendLine(GetId(item));
            details.AppendLine(GetName(item));

            if (!string.IsNullOrWhiteSpace(item.Summary))
                details.AppendLine(GetSummary(item));

            if (item.Quotes.Count > 0)
                details.AppendLine(GetQuotes(item));

            details.AppendLine(GetRarity(item));
            details.AppendLine(GetTags(item));

            if (item.Value > 0)
            {
                details.AppendLine(GetValue(item));
                details.AppendLine(GetBuyState(item));
                details.AppendLine(GetSellState(item));
            }

            details.AppendLine(GetTradeState(item));
            details.AppendLine(GetGiftState(item));
            details.AppendLine(GetBypassState(item));
            details.AppendLine(GetUseState(item));
            details.AppendLine(GetUniqueState(item));

            details.AppendLine(GetOwnLimit(item));

            return details.ToString();
        }
    }
}