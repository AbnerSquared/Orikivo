using Discord;
using Orikivo.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents the management of all purchases.
    /// </summary>
    public class ShopSystem
    {
        public List<IShop> Shops { get; set; }
        public static MessageBuilder CheckStores()
        {
            MessageBuilder mb = new MessageBuilder();
            string shop_sprite = ".//resources//tmp_shops.png";
            EmbedBuilder eb = Embedder.DefaultEmbed;

            // for each store
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Schematrix [OPEN]"); // The schema shop for profiles
            sb.AppendLine("Pocket Residence [OPEN]"); // Mr. Pocket's home location.

            eb.WithDescription(sb.ToString());
            eb.WithLocalImageUrl(shop_sprite);
            mb.WithFile(shop_sprite);
            mb.WithEmbed(eb);

            return mb;
        }
    }

    // center => places that offer rewards/perform specific tasks.
    // shop => places that can buy/sell.

    public class OriShopTimeBlock
    {
        public int OpeningHour { get; set; } // hour from 0:00 to 23:00
        public int ClosingHour { get; set; }

        private TimeSpan EntryTime { get; set; }
        private TimeSpan ExitTime { get; set; } // times in comparison to DateTime.UtcNow
    }


    // used to define a time block.
    public class DateTimeRange
    {
        // the point of the shop opening
        TimeSpan EntryPoint { get; set; }

        // the poing of the shop closing.
        TimeSpan ExitPoint { get; set; }

        // however long the shop is open.
        public TimeSpan Duration
        {
            get
            {
                return ExitPoint < EntryPoint ? (TimeSpan.FromHours(24) - EntryPoint) + ExitPoint : ExitPoint - EntryPoint;
            }
        }

        public DateTime OpeningHour { get; set; }
        public DateTime ClosingHour { get; set; }

        public bool IsOpen
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                return OpeningHour <= now && now < ClosingHour;
            }
        }
    }

    // the basis of all shop systems.
    public interface IShop
    {

    }
}
