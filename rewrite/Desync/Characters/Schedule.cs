using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents an availability of time.
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// A collection of shifts for the following <see cref="Schedule"/>.
        /// </summary>
        public List<Shift> Shifts { get; set; }

        /// <summary>
        /// Gets a double-precision floating-point number that represents the total amount of hours that the <see cref="Schedule"/> takes.
        /// </summary>
        public double Length => Shifts.Sum(x => x.Length.TotalHours);

        /// <summary>
        /// Returns a <see cref="bool"/> that defines if the <see cref="Schedule"/> will be active at the given <see cref="DateTime"/>.
        /// </summary>
        public bool IsAvailable(DateTime time)
        {
            return GetNextBlock(time).Contains(time);
        }

        /// <summary>
        /// Returns a <see cref="bool"/> that defines if the <see cref="Schedule"/> is currently active as of <see cref="DateTime.UtcNow"/>.
        /// </summary>
        public bool IsActive()
        {
            return GetNextBlock(DateTime.UtcNow).Contains(DateTime.UtcNow);
        }

        /// <summary>
        /// Returns a <see cref="bool"/> that defines if the <see cref="Schedule"/> is active for the current <see cref="Shift"/> at the specified <see cref="DateTime"/>.
        /// </summary>
        public bool IsActive(DateTime time)
        {
            return GetNextBlock(DateTime.UtcNow).Contains(time);
        }

        /// <summary>
        /// Returns the next available <see cref="TimeBlock"/> at the specified <see cref="DateTime"/>.
        /// </summary>
        public TimeBlock GetNextBlock(DateTime time)
        {
            DayOfWeek day = time.DayOfWeek;

            if (Shifts.Count == 0)
                throw new Exception("There must at least be one shift specified.");
            int dayOffset = 0;

            // force offset if the shift is on the same day, but it already ended.
            if (Shifts.Any(x => x.Day == day))
            {
                Shift last = Shifts.First(x => x.Day == day);
                DateTime lastFrom = time.Date.AddHours(last.StartingHour).AddMinutes(last.StartingMinute);
                DateTime lastTo = lastFrom.Add(last.Length);

                TimeBlock lastBlock = new TimeBlock(lastFrom, lastTo);

                // if the last shift is out of range, force another day so it can read the next upcoming shifts.
                if (!lastBlock.IsBefore(time) && !lastBlock.Contains(time))
                {
                    day = (DayOfWeek)((int)(day + 1) % 7);
                    dayOffset++;
                }
                else
                    return lastBlock;
            }


            while(!Shifts.Any(x => x.Day == day))
            {
                day = (DayOfWeek)((int)(day + 1) % 7);
                dayOffset++;
            }

            Shift next = Shifts.First(x => x.Day == day);

            DateTime from = time.Date.AddDays(dayOffset)
                .AddHours(next.StartingHour).AddMinutes(next.StartingMinute);

            DateTime to = from.Add(next.Length);

            Console.WriteLine(from.ToLongTimeString());
            Console.WriteLine(to.ToLongTimeString());

            return new TimeBlock(from, to);
        }
    }
}
