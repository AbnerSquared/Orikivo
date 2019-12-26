﻿namespace Orikivo
{
    /// <summary>
    /// A configuration class that control how events are logged.
    /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// Gets the default <see cref="LogConfig"/>.
        /// </summary>
        public static LogConfig Default
        {
            get
            {
                LogConfig config = new LogConfig
                {
                    Colors = LogColorMap<Discord.LogSeverity>.Discord,
                    Aliases = LogAliasMap.Default
                };

                // TODO: Allow color mapping on aliases
                config.EntryFormat = $"[{config.Aliases[LogAlias.Date]}] {config.Aliases[LogAlias.Name]}\n{config.Aliases[LogAlias.LogSeverity]}.{config.Aliases[LogAlias.LogSource]}";
                config.ExceptionFormat = $"Oops!\nAn exception has occured. [{config.Aliases[LogAlias.ExceptionType]}]\n\n{config.Aliases[LogAlias.ExceptionMessage]}";
                config.MessageFormat = $"{config.Aliases[LogAlias.LogMessage]}";
                config.ExitFormat = "-- --";
                return config;
            }
        }
        
        /// <summary>
        /// The <see cref="LogAliasMap"/> that defines the shortcuts for the specified <see cref="LogAlias"/> values.
        /// </summary>
        public LogAliasMap Aliases { get; set; }

        /// <summary>
        /// The <see cref="LogColorMap{TEnum}"/> that defines the <see cref="System.ConsoleColor"/> pair for the specified key.
        /// </summary>
        public LogColorMap<Discord.LogSeverity> Colors { get; set; }

        /// <summary>
        /// The formatter value to be used when a log event is first called.
        /// </summary>
        public string EntryFormat { get; set; }

        /// <summary>
        /// The formatter value to be used when an exception occurs during execution.
        /// </summary>
        public string ExceptionFormat { get; set; }

        /// <summary>
        /// The formatter value to be used when a generic log message is written.
        /// </summary>
        public string MessageFormat { get; set; }

        /// <summary>
        /// This is the formatter value used when a log event is closing.
        /// </summary>
        public string ExitFormat { get; set; }
    }
}
