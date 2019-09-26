using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class OriLogColorConfig
    {
        public OriLogColorConfig()
        {
            LogColors = new Dictionary<LogSeverity, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?>();
        }
        // this manages log severity colors
        public static OriLogColorConfig Default
        {
            get
            {
                OriLogColorConfig logColorConfig = new OriLogColorConfig();
                logColorConfig.LogColors.Add(LogSeverity.Debug, (null, ConsoleColor.Cyan));
                logColorConfig.LogColors.Add(LogSeverity.Error, (null, ConsoleColor.DarkRed));
                logColorConfig.LogColors.Add(LogSeverity.Warning, (null, ConsoleColor.Yellow));
                logColorConfig.LogColors.Add(LogSeverity.Info, (null, ConsoleColor.White));
                logColorConfig.LogColors.Add(LogSeverity.Critical, (null, ConsoleColor.DarkMagenta));
                logColorConfig.LogColors.Add(LogSeverity.Verbose, (null, null));
                return logColorConfig;
            }
        }
        private Dictionary<LogSeverity, (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)?> LogColors { get; set; }

        public void SetLogColors(LogSeverity logSeverity, ConsoleColor? backgroundColor, ConsoleColor? textColor)
        {
            LogColors[logSeverity] = (backgroundColor, textColor);
        }

        public (ConsoleColor? BackgroundColor, ConsoleColor? TextColor)? this[LogSeverity logSeverity]
        {
            get
            {
                return LogColors[logSeverity] ?? Default.LogColors[logSeverity] ?? null;
            }
        }
    }
}
