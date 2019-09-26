using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class OriLoggerConfig
    {
        public static OriLoggerConfig Default
        {
            get
            {
                // NAME = name
                // DATE = date
                // MESSAGE = message, if any
                // LOG_SEVERITY
                // LOG_SOURCE
                // EX_TYPE
                // EX_REASON
                OriLoggerConfig loggerConfig = new OriLoggerConfig();
                loggerConfig.LogColors = OriLogColorConfig.Default;
                loggerConfig.LogKeyEntry = "{";
                loggerConfig.LogKeyExit = "}";
                loggerConfig.LogKeys = OriLogKeyConfig.Default;
                loggerConfig.EntryFormat = $"-- Log Entry --\n{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.Name]}{loggerConfig.LogKeyExit} [{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.Date]}{loggerConfig.LogKeyExit}]\n{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.LogSeverity]}{loggerConfig.LogKeyExit}.{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.LogSource]}{loggerConfig.LogKeyExit}";
                loggerConfig.ExceptionFormat = $"~ An exception has occured. [{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.ExceptionType]}{loggerConfig.LogKeyExit}] ~\n{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.ExceptionMessage]}{loggerConfig.LogKeyExit}";
                loggerConfig.MessageFormat = $"{loggerConfig.LogKeyEntry}{loggerConfig.LogKeys[LogKeyType.LogMessage]}{loggerConfig.LogKeyExit}";
                loggerConfig.ExitFormat = "-- Log Exit --";
                return loggerConfig;
            }
        }
        public string LogKeyEntry { get; set; } // the string that identifies when to open a log key value
        public string LogKeyExit { get; set; } // the string that identifies when to close a log key value
        public OriLogKeyConfig LogKeys { get; set; }
        public OriLogColorConfig LogColors { get; set; }
        public string EntryFormat { get; set; } // displays the name, date, and other in that category.
        public string ExceptionFormat { get; set; } // used to display the exception that occured, when an error occured/
        public string MessageFormat { get; set; } // used to display how a message is shown
        public string ExitFormat { get; set; } // used to close a log message
    }
}
