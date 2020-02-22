using System;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a range for two <see cref="DateTime"/> values.
    /// </summary>
    public struct TimeBlock
    {
        /// <summary>
        /// Constructs a new <see cref="TimeBlock"/> starting from <see cref="DateTime.UtcNow"/> offset by a specified <see cref="TimeSpan"/>.
        /// </summary>
        public TimeBlock(TimeSpan length)
        {
            From = DateTime.UtcNow;
            To = From.Add(length);
        }

        /// <summary>
        /// Constructs a new <see cref="TimeBlock"/> starting from a specified <see cref="DateTime"/> offset by a specified <see cref="TimeSpan"/>.
        /// </summary>
        public TimeBlock(DateTime from, TimeSpan length)
        {
            From = from;
            To = from.Add(length);
        }

        /// <summary>
        /// Constructs a new <see cref="TimeBlock"/> starting and ending from the specified <see cref="DateTime"/> values.
        /// </summary>
        public TimeBlock(DateTime from, DateTime to)
        {
            if (to <= from)
                throw new ArgumentException("The closing time must be after the starting time.");


            From = from;
            To = to;
        }

        /// <summary>
        /// Returns a new <see cref="TimeBlock"/> in which its values are copied.
        /// </summary>
        public TimeBlock Clone()
        {
            return new TimeBlock(From, To);
        }

        /// <summary>
        /// The <see cref="DateTime"/> at which this <see cref="TimeBlock"/> starts.
        /// </summary>
        public DateTime From { get; private set; }

        /// <summary>
        /// The <see cref="DateTime"/> at which this <see cref="TimeBlock"/> ends.
        /// </summary>
        public DateTime To { get; private set; }

        /// <summary>
        /// Returns the duration of this <see cref="TimeBlock"/>.
        /// </summary>
        public TimeSpan Length => To - From;

        /// <summary>
        /// Returns a <see cref="bool"/> defining if the <see cref="TimeBlock"/> contains the specified <see cref="DateTime"/>.
        /// </summary>
        public bool Contains(DateTime time)
        {
            return From <= time && time <= To;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> defining if the specified <see cref="DateTime"/> is before the <see cref="TimeBlock"/>.
        /// </summary>
        public bool IsBefore(DateTime time)
        {
            return From >= time;
        }

        /// <summary>
        /// Returns a new <see cref="TimeBlock"/> in which its entries are offset by the specified <see cref="TimeSpan"/>.
        /// </summary>
        public TimeBlock Offset(TimeSpan time)
        {
            TimeBlock next = Clone();

            next.From = From.Add(time);
            next.To = To.Add(time);

            return next;
        }
    }
}
