using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// A map defining the keys of a log key. This is used primarily when logging events or writing to a console.
    /// </summary>
    public class LogKeyMap
    {
        private const char ENTRY = '{';
        private const char EXIT = '}';

        public LogKeyMap()
        {
            Keys = new Dictionary<LogKey, string>();
        }

        /// <summary>
        /// Gets the default log key map that is used with Orikivo.
        /// </summary>
        public static LogKeyMap Default
        {
            get
            {
                LogKeyMap map = new LogKeyMap();
                map.Keys = new Dictionary<LogKey, string>
                {
                    [LogKey.Name] = "name",
                    [LogKey.Date] = "date" ,
                    [LogKey.LogMessage] = "message",
                    [LogKey.Exception] = "ex",
                    [LogKey.ExceptionType] = "ex_type",
                    [LogKey.ExceptionMessage] = "ex_message",
                    [LogKey.LogSeverity] = "log_severity",
                    [LogKey.LogSource] = "log_source",
                    [LogKey.ClientVersion] = "client_version"
                };
                return map;
            }
        }

        /// <summary>
        /// Sets the specified log key to match with the specified key.
        /// </summary>
        /// <param name="type">The log key to set.</param>
        /// <param name="key">The new name of the specified log key.</param>
        public void SetKey(LogKey type, string key)
        {
            if (!Checks.NotNull(key))
                throw new NullReferenceException("A log key must have a name that is not empty.");
            Keys[type] = key;
        }
        
        /// <summary>
        /// The collection of log keys set.
        /// </summary>
        private Dictionary<LogKey, string> Keys { get; set; }

        /// <summary>
        /// Gets the log key formatting string from the specified log key. If there isn't a log key specified, it defaults to LogKeyMap.Default[LogKey].
        /// </summary>
        /// <param name="type">The log key to get the format for.</param>
        /// <returns></returns>
        public string this[LogKey type] => $"{ENTRY}{Keys[type] ?? Default.Keys[type]}{EXIT}";
    }
}
