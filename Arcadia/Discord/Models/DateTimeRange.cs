using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a range for two <see cref="DateTime"/> values.
    /// </summary>
    public readonly struct DateTimeRange
    {
        /// <summary>
        /// Initializes a new <see cref="DateTimeRange"/> starting from <see cref="DateTime.UtcNow"/> offset by a specified <see cref="TimeSpan"/>.
        /// </summary>
        public DateTimeRange(TimeSpan length)
        {
            From = DateTime.UtcNow;
            To = From.Add(length);
        }

        /// <summary>
        /// Initializes a new <see cref="DateTimeRange"/> starting from a specified <see cref="DateTime"/> offset by a specified <see cref="TimeSpan"/>.
        /// </summary>
        public DateTimeRange(DateTime from, TimeSpan length)
        {
            From = from;
            To = from.Add(length);
        }

        /// <summary>
        /// Initializes a new <see cref="DateTimeRange"/> starting and ending from the specified <see cref="DateTime"/> values.
        /// </summary>
        public DateTimeRange(DateTime from, DateTime to)
        {
            if (to <= from)
                throw new ArgumentException("The ending time must be greater than the starting time.");


            From = from;
            To = to;
        }

        /// <summary>
        /// Returns a new <see cref="DateTimeRange"/> in which its values are copied.
        /// </summary>
        public DateTimeRange Clone()
        {
            return new DateTimeRange(From, To);
        }

        /// <summary>
        /// The <see cref="DateTime"/> at which this <see cref="DateTimeRange"/> starts.
        /// </summary>
        public DateTime From { get; }

        /// <summary>
        /// The <see cref="DateTime"/> at which this <see cref="DateTimeRange"/> ends.
        /// </summary>
        public DateTime To { get; }

        /// <summary>
        /// Returns the duration of this <see cref="DateTimeRange"/>.
        /// </summary>
        public TimeSpan Length => To - From;

        /// <summary>
        /// Returns a <see cref="bool"/> defining if the <see cref="DateTimeRange"/> contains the specified <see cref="DateTime"/>.
        /// </summary>
        public bool Contains(DateTime time)
        {
            return From <= time && time <= To;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> defining if the specified <see cref="DateTime"/> is before the <see cref="DateTimeRange"/>.
        /// </summary>
        public bool IsBefore(DateTime time)
        {
            return From >= time;
        }

        /// <summary>
        /// Returns a new <see cref="DateTimeRange"/> in which its entries are offset by the specified <see cref="TimeSpan"/>.
        /// </summary>
        public DateTimeRange Offset(TimeSpan time)
        {
            return new DateTimeRange(From.Add(time), To.Add(time));
        }
    }
}
