using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public class OriDiscordConfig
    {
        // make discord config separate into their own variables, and create configs when done
        public static DiscordSocketConfig DefaultSocketConfig
        {
            get
            {
                DiscordSocketConfig socketConfig = new DiscordSocketConfig();
                socketConfig.AlwaysDownloadUsers = true;
                socketConfig.LogLevel = LogSeverity.Info;//LogSeverity.Verbose;
                socketConfig.MessageCacheSize = 100;
                socketConfig.LargeThreshold = 250;
                return socketConfig;
            }
        }

        public static CommandServiceConfig DefaultServiceConfig
        {
            get
            {
                CommandServiceConfig serviceConfig = new CommandServiceConfig();
                serviceConfig.CaseSensitiveCommands = false;
                serviceConfig.DefaultRunMode = RunMode.Async;
                serviceConfig.LogLevel = LogSeverity.Info;//LogSeverity.Verbose;
                serviceConfig.ThrowOnError = true;
                return serviceConfig;
            }
        }
    }
}
