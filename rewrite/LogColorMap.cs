using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A map defining the colors of a log severity. This is used primarily when logging Discord-related events.
    /// </summary>
    public class LogColorMap
    {
        public LogColorMap()
        {
            Colors = new Dictionary<LogSeverity, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?>();
        }

        /// <summary>
        /// Gets the default log color map that is used with Orikivo.
        /// </summary>
        public static LogColorMap Default
        {
            get
            {
                LogColorMap map = new LogColorMap();
                map.Colors = new Dictionary<LogSeverity, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?>()
                {
                    [LogSeverity.Debug] = (null, ConsoleColor.Cyan),
                    [LogSeverity.Error] = (null, ConsoleColor.DarkRed),
                    [LogSeverity.Warning] = (null, ConsoleColor.Yellow),
                    [LogSeverity.Info] = (null, ConsoleColor.White),
                    [LogSeverity.Critical] = (null, ConsoleColor.DarkMagenta),
                    [LogSeverity.Verbose] = (null, ConsoleColor.Gray)
                };
                return map;
            }
        }

        private Dictionary<LogSeverity, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?> Colors { get; set; }

        /// <summary>
        /// Sets the new color palette to use on a specified log severity.
        /// </summary>
        public void SetColors(LogSeverity severity, ConsoleColor? backgroundColor, ConsoleColor? textColor)
        => Colors[severity] = (backgroundColor, textColor);

        /// <summary>
        /// Gets the color palette set for the specified log severity. If there is not one set, it defaults to LogColorMap.Default[LogSeverity].
        /// </summary>
        /// <param name="severity">The log severity to get the color palette for.</param>
        public (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)? this[LogSeverity severity]
            => Colors[severity] ?? Default.Colors[severity] ?? null;
    }
}
