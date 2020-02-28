using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents the configuration for an <see cref="Client"/>.
    /// </summary>
    public class DiscordConfig
    {
        // make discord config separate into their own variables, and create configs when done
        public static DiscordSocketConfig DefaultSocketConfig
        {
            get
            {
                DiscordSocketConfig socketConfig = new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Info, //LogSeverity.Verbose;
                    MessageCacheSize = 100,
                    LargeThreshold = 250
                };
                return socketConfig;
            }
        }

        public static CommandServiceConfig DefaultServiceConfig
        {
            get
            {
                CommandServiceConfig serviceConfig = new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Info, //LogSeverity.Verbose;
                    ThrowOnError = true, 
                    //QuotationMarkAliasMap = null
                };
                return serviceConfig;
            }
        }
    }
}
