using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class OriLogConfig
    {
        public static OriLogConfig Default
        {
            get
            {
                OriLogConfig config = new OriLogConfig
                {
                    Colors = LogColorMap.Default,
                    Keys = LogKeyMap.Default
                };
                config.EntryFormat = $"--  --\n{config.Keys[LogKey.Name]} [{config.Keys[LogKey.Date]}]\n{config.Keys[LogKey.LogSeverity]}.{config.Keys[LogKey.LogSource]}";
                config.ExceptionFormat = $"~ An exception has occured. [{config.Keys[LogKey.ExceptionType]}] ~\n{config.Keys[LogKey.ExceptionMessage]}";
                config.MessageFormat = $"{config.Keys[LogKey.LogMessage]}";
                config.ExitFormat = "-- --";
                return config;
            }
        }
        
        public LogKeyMap Keys { get; set; }
        public LogColorMap Colors { get; set; }
        public string EntryFormat { get; set; } // displays the name, date, and other in that category.
        public string ExceptionFormat { get; set; } // used to display the exception that occured, when an error occured/
        public string MessageFormat { get; set; } // used to display how a message is shown
        /// <summary>
        /// This is the message that is shown when a log is closing.
        /// </summary>
        public string ExitFormat { get; set; } // used to close a log message
    }
}
