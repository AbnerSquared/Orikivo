using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a <see cref="Construct"/> as a shop.
    /// </summary>
    public class Market : Construct
    {
        public Market()
        {
            Tag = ConstructType.Market;
            CanUseDecor = false;
        }

        public Market(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Represents the generation information for the <see cref="Market"/>.
        /// </summary>
        public CatalogGenerator Catalog { get; set; }
        
        public List<Vendor> Vendors { get; set; }

        private List<Character> _npcs;


        public override List<Character> Npcs
        {
            get
            {
                if (IsActive())
                {
                    var npcs = new List<Character>{ GetActive() };
                    npcs.AddRange(_npcs);
                    return npcs;
                }

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
            var schedule = new StringBuilder();
            schedule.AppendLine($"**Schedule** ({Name}):");
            foreach (Vendor vendor in Vendors)
            {
                if (!(vendor.Schedule.Length > 0))
                    continue;

                schedule.AppendLine($"**{vendor.Name}**");
                foreach(Shift shift in vendor.Schedule.Shifts.OrderBy(x => x.Day))
                {
                    schedule.Append($"> **{shift.Day}**: ");
                    schedule.Append($"**{(shift.StartingHour):00}:{(shift.StartingMinute):00}**-");
                    int minutes = shift.StartingMinute + shift.Length.Minutes;
                    schedule.Append($"**{(shift.StartingHour + shift.Length.Hours + Math.Floor(minutes / (double) 60)):00}:{(minutes % 60):00}**");
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

        // Get schedule from Vendor shifts.
    }
}
