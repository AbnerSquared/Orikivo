using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // used to edit what should be considered the log key
    public class OriLogKeyConfig
    {
        public OriLogKeyConfig()
        {
            LogKeys = new Dictionary<LogKeyType, string>();
        }

        public static OriLogKeyConfig Default
        {
            get
            {
                OriLogKeyConfig logKeyConfig = new OriLogKeyConfig();
                logKeyConfig.LogKeys = new Dictionary<LogKeyType, string>();
                logKeyConfig.LogKeys.Add(LogKeyType.Name, "name");
                logKeyConfig.LogKeys.Add(LogKeyType.Date, "date");
                logKeyConfig.LogKeys.Add(LogKeyType.LogMessage, "message");
                logKeyConfig.LogKeys.Add(LogKeyType.Exception, "ex");
                logKeyConfig.LogKeys.Add(LogKeyType.ExceptionType, "ex_type");
                logKeyConfig.LogKeys.Add(LogKeyType.ExceptionMessage, "ex_message");
                logKeyConfig.LogKeys.Add(LogKeyType.LogSeverity, "log_severity");
                logKeyConfig.LogKeys.Add(LogKeyType.LogSource, "log_source");
                logKeyConfig.LogKeys.Add(LogKeyType.ClientVersion, "client_version");
                return logKeyConfig;
            }
        }

        public void SetLogKey(LogKeyType keyType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new NullReferenceException("The value used to set a LogKey is null.");
            LogKeys[keyType] = value;
        }

        private Dictionary<LogKeyType, string> LogKeys { get; set; }
        public string this[LogKeyType keyType]
        {
            get { return LogKeys[keyType] ?? Default.LogKeys[keyType]; }
        }
    }
}
