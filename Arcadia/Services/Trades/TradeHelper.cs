using System.Collections.Generic;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public static class TradeHelper
    {
        public const string Separator = "for";

        public static string ViewOffers(ArcadeUser user)
        {
            var info = new StringBuilder();

            info.AppendLine($"> **Trade Offers**");
            info.AppendLine($"> Because connections matter.");

            info.AppendLine($"\nYou do not have any pending offers.");

            return info.ToString();
        }

        public static string ViewOffer(TradeOffer offer)
        {
            var info = new StringBuilder();
            return info.ToString();
        }

        public static TradeOffer ParseOffer(ulong userId, string input)
        {
            var reader = new StringReader(input);

            // TODO: parse trade offer input here

            var offered = new Dictionary<string, int>();
            var requested = new Dictionary<string, int>();

            return new TradeOffer(userId)
            {
                ItemIds = offered,
                RequestedItemIds = requested
            };
        }
    }
}