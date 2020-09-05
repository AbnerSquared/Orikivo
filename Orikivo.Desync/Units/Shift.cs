using System;

namespace Orikivo.Desync
{
    // use a 24-hour format.
    /// <summary>
    /// Represents a <see cref="TimeBlock"/> configuration for a <see cref="Schedule"/>.
    /// </summary>
    public class Shift
    {
        public Shift(DayOfWeek day, int startingHour, int startingMinute, TimeSpan length)
        {
            Day = day;

            if (startingHour < 0 || startingHour > 23)
                throw new ArgumentException("The specified hour is out of range.");
            if (startingMinute < 0 || startingMinute > 59)
                throw new ArgumentException("The specified minute is out of range.");
            if (length <= TimeSpan.Zero)
                throw new ArgumentException("The specified length must be greater than 0.");

            StartingHour = startingHour;
            StartingMinute = startingMinute;

            Length = length;
            
        }

        /// <summary>
        /// The <see cref="DayOfWeek"/> that this <see cref="Shift"/> takes place on.
        /// </summary>
        public DayOfWeek Day { get; }
        
        /// <summary>
        /// The starting hour of this <see cref="Shift"/> (0-23).
        /// </summary>
        public int StartingHour { get; }

        /// <summary>
        /// The starting minute of this <see cref="Shift"/> (0-59).
        /// </summary>
        public int StartingMinute { get; }

        //Cannot be more than 24 hours.
        /// <summary>
        /// The duration of this <see cref="Shift"/>.
        /// </summary>
        public TimeSpan Length { get; }
    }
}
