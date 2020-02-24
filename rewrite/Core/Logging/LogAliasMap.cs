using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// A map that points specified shortcut values to a specific <see cref="LogAlias"/>.
    /// </summary>
    public class LogAliasMap
    {
        private const char ENTRY_CHAR = '{';
        private const char EXIT_CHAR = '}';

        /// <summary>
        /// Creates a new <see cref="LogAliasMap"/>.
        /// </summary>
        public LogAliasMap()
        {
            Aliases = new Dictionary<LogAlias, string>();
        }

        /// <summary>
        /// Gets the default <see cref="LogAliasMap"/>.
        /// </summary>
        public static LogAliasMap Default
        {
            get
            {
                return new LogAliasMap
                {
                    Aliases = new Dictionary<LogAlias, string>
                    {
                        [LogAlias.Name] = "name",
                        [LogAlias.Date] = "date",
                        [LogAlias.LogMessage] = "message",
                        [LogAlias.Exception] = "ex",
                        [LogAlias.ExceptionType] = "ex_type",
                        [LogAlias.ExceptionMessage] = "ex_message",
                        [LogAlias.LogSeverity] = "log_severity",
                        [LogAlias.LogSource] = "log_source",
                        [LogAlias.ClientVersion] = "client_version"
                    }
                };
            }
        }

        private Dictionary<LogAlias, string> Aliases { get; set; }

        /// <summary>
        /// Sets a shortcut value that points to the specified <see cref="LogAlias"/>.
        /// </summary>
        /// <param name="alias">The <see cref="LogAlias"/> to map a shortcut value at.</param>
        /// <param name="shortcut">The shortcut value that refers to the specified <see cref="LogAlias"/>.</param>
        public void SetAlias(LogAlias alias, string shortcut)
        {
            if (!Check.NotNull(shortcut))
                throw new NullReferenceException("The shortcut specified was empty.");

            Aliases[alias] = shortcut;
        }

        /// <summary>
        /// Gets the shortcut value written for the specified <see cref="LogAlias"/>. If there isn't a shortcut specified, it returns the shortcut specified at <see cref="Default"/>.
        /// </summary>
        /// <param name="type">The <see cref="LogAlias"/> to get the shortcut value at.</param>
        public string this[LogAlias type] => $"{ENTRY_CHAR}{Aliases[type] ?? Default.Aliases[type]}{EXIT_CHAR}";
    }
}
