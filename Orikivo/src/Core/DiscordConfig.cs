using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Orikivo
{
    public static class DiscordConfig
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
                    LargeThreshold = 250,
                    RateLimitPrecision = RateLimitPrecision.Millisecond
                };
                return socketConfig;
            }
        }

        public static CommandServiceConfig DefaultCommandConfig
        {
            get
            {
                CommandServiceConfig serviceConfig = new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Info, //LogSeverity.Verbose;
                    ThrowOnError = true
                    //QuotationMarkAliasMap = null
                };
                return serviceConfig;
            }
        }
    }
}
