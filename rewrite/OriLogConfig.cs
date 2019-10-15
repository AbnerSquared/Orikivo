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
                const char ENT = '{';
                const char EX = '}';

                OriLogConfig config = new OriLogConfig
                {
                    Colors = LogColorMap.Default,
                    Keys = LogKeyMap.Default
                };
                config.EntryFormat = $"-- ~ --\n{ENT}{config.Keys[LogKey.Name]}{EX} [{ENT}{config.Keys[LogKey.Date]}{EX}]\n{ENT}{config.Keys[LogKey.LogSeverity]}{EX}.{ENT}{config.Keys[LogKey.LogSource]}{EX}";
                config.ExceptionFormat = $"~ An exception has occured. [{ENT}{config.Keys[LogKey.ExceptionType]}{EX}] ~\n{ENT}{config.Keys[LogKey.ExceptionMessage]}{EX}";
                config.MessageFormat = $"{ENT}{config.Keys[LogKey.LogMessage]}{EX}";
                config.ExitFormat = "-- END --";
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
