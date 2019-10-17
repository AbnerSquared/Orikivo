using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // used to edit what should be considered the log key
    public class LogKeyMap
    {
        public LogKeyMap()
        {
            Keys = new Dictionary<LogKey, string>();
        }

        public static LogKeyMap Default
        {
            get
            {
                LogKeyMap config = new LogKeyMap();
                config.Keys = new Dictionary<LogKey, string>
                {
                    { LogKey.Name, "name" },
                    { LogKey.Date, "date" },
                    { LogKey.LogMessage, "message" },
                    { LogKey.Exception, "ex" },
                    { LogKey.ExceptionType, "ex_type" },
                    { LogKey.ExceptionMessage, "ex_message" },
                    { LogKey.LogSeverity, "log_severity" },
                    { LogKey.LogSource, "log_source" },
                    { LogKey.ClientVersion, "client_version" }
                };
                return config;
            }
        }

        public void SetLogKey(LogKey keyType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new NullReferenceException("The value used to set a LogKey is null.");
            Keys[keyType] = value;
        }
        private const char ENT = '{';
        private const char EX = '}';
        private Dictionary<LogKey, string> Keys { get; set; }
        public string this[LogKey keyType] => $"{ENT}{Keys[keyType] ?? Default.Keys[keyType]}{EX}";
    }
}
