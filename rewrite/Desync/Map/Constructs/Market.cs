using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Unstable
{
    // have a handler when you try to enter a market and its closed.
    public class Market : Construct
    {
        // this is the image that is used as the background for vendors.
        public Sprite Interior { get; set; }
        public LootTable Table { get; set; } // the tags of the groups of items that it can sell.
        
        // a list of workers that work here
        public List<Vendor> Vendors { get; set; }

        private List<Npc> _npcs;


        public override List<Npc> Npcs
        {
            get
            {
                if (IsActive())
                {
                    List<Npc> npcs = ((Npc)GetActive()).AsList();
                    npcs.AddRange(_npcs);
                    return npcs;
                }
                else
                    return _npcs;
            }
            set => _npcs = value;
        }

        // marked true if the market supports the buying of items
        public bool CanBuyFrom { get; set; }

        // marked true if the markets supports the selling of items
        public bool CanSellFrom { get; set; }
        
        // a multiplier applied to items that are sold
        public float SellRate { get; set; }

        // formats a string in which a schedule is formatted.
        public string ShowSchedule()
        {
            StringBuilder schedule = new StringBuilder();
            schedule.AppendLine($"**Schedule** ({Name}):");
            foreach (Vendor vendor in Vendors)
            {
                if (!(vendor.Schedule.Length > 0))
                    continue;

                schedule.AppendLine($"**{vendor.Name}**");
                foreach(Shift shift in vendor.Schedule.Shifts.OrderBy(x => x.Day))
                {
                    schedule.Append($"> **{shift.Day}**: ");
                    schedule.Append($"**{(shift.StartingHour).ToString("00")}:{(shift.StartingMinute).ToString("00")}**-");
                    int minutes = shift.StartingMinute + shift.Length.Minutes;
                    schedule.Append($"**{(shift.StartingHour + shift.Length.Hours + Math.Floor((double)(minutes / 60))).ToString("00")}:{(minutes % 60).ToString("00")}**");
                    schedule.AppendLine();
                }
            }

            return schedule.ToString();
        }

        // returns a bool showing if the market is currently open.
        public bool IsActive()
            => Vendors.Any(x => x.Schedule.IsActive());

        public bool IsAvailable(DateTime time)
            => Vendors.Any(x => x.Schedule.IsAvailable(time));

        // gets the currently active vendor.
        public Vendor GetActive()
            => IsActive() ? Vendors.First(x => x.Schedule.IsActive()) : null;

        // get the vendor that will be available at that time.
        public Vendor GetAvailable(DateTime time)
            => Vendors.First(x => x.Schedule.IsAvailable(time));

        // checks if a time is synced up with the currently active shift
        public bool IsActive(DateTime time)
            => Vendors.Any(x => x.Schedule.IsActive(time));

        /// <summary>
        /// Returns the next available <see cref="TimeBlock"/> at the specified <see cref="DateTime"/>.
        /// </summary>
        public TimeBlock GetNextBlock(DateTime time) // TODO: Optimize.
            => Vendors.Select(x => x.Schedule.GetNextBlock(time)).Where(x => (x.From - time).TotalSeconds > 0).OrderBy(x => x.From - time).First();

        // creates a new list of items to sell for the day
        public MarketCatalog GenerateCatalog()
        {
            IEnumerable<Item> loot = WorldEngine.Items
                .Where(x => Table.Groups?.Any(t => x.Value.Tag.HasFlag(t)) ?? true)
                .Where(x => x.Value.Tag.HasFlag(Table.RequiredTags))
                .Select(x => x.Value);

            List<Item> catalog = Table.MaxStack <= 0 ? Randomizer.ChooseMany(loot, Table.Capacity, true).ToList() :
                Randomizer.ChooseMany(loot, Table.Capacity, Table.MaxStack).ToList();

            Dictionary<Item, int> items = new Dictionary<Item, int>();

            foreach (Item item in catalog)
            {
                if (items.ContainsKey(item))
                    continue;

                items.Add(item, catalog.Where(x => x == item).Count());
            }

            return new MarketCatalog(items);
        }

        // Get schedule from Vendor shifts.
    }
}
