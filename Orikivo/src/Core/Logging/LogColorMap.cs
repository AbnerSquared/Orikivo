using System;
using System.Collections.Generic;
using LogSeverity = Discord.LogSeverity;

namespace Orikivo
{
    /// <summary>
    /// A map containing a pair of <see cref="ConsoleColor"/> values for each <see cref="TEnum"/> key specified.
    /// </summary>
    public class LogColorMap<TEnum> where TEnum : struct
    {
        /// <summary>
        /// Creates a new <see cref="LogColorMap{TEnum}"/>.
        /// </summary>
        public LogColorMap()
        {
            //if (typeof(TEnum) != typeof(Enum))
            //    throw new InvalidCastException("Type TEnum cannot cast to type Enum.");

            Colors = new Dictionary<TEnum, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?>();
        }

        /// <summary>
        /// The default color map used with <see cref="LogSeverity"/>.
        /// </summary>
        public static LogColorMap<LogSeverity> Discord
        {
            get
            {
                return new LogColorMap<LogSeverity>
                {
                    Colors = new Dictionary<LogSeverity, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?>()
                    {
                        [LogSeverity.Debug] = (null, ConsoleColor.Cyan),
                        [LogSeverity.Error] = (null, ConsoleColor.DarkRed),
                        [LogSeverity.Warning] = (null, ConsoleColor.Yellow),
                        [LogSeverity.Info] = (null, ConsoleColor.White),
                        [LogSeverity.Critical] = (null, ConsoleColor.DarkMagenta),
                        [LogSeverity.Verbose] = (null, ConsoleColor.Gray)
                    }
                };
            }
        }

        private Dictionary<TEnum, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?> Colors { get; set; }

        /// <summary>
        /// Sets a new <see cref="ConsoleColor"/> pair to a specified <see cref="TEnum"/> value.
        /// </summary>
        public void SetColorPair(TEnum logLevel, ConsoleColor? backgroundColor = null, ConsoleColor? textColor = null)
            => Colors[logLevel] = (backgroundColor == null && textColor == null) ? ((ConsoleColor?, ConsoleColor?)?)null : (backgroundColor, textColor);

        /// <summary>
        /// Gets the <see cref="ConsoleColor"/> pair to use for the specified <see cref="TEnum"/>.
        /// </summary>
        public (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)? this[TEnum logLevel]
            => Colors[logLevel] ?? null;
    }
}
