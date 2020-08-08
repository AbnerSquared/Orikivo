using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Orikivo
{
    public static class DiscordConfig
    {
        public static DiscordSocketConfig DefaultSocketConfig
        {
            get
            {
                return new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 100,
                    LargeThreshold = 250,
                    RateLimitPrecision = RateLimitPrecision.Millisecond,
                    //GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.DirectMessageReactions
                };
            }
        }

        public static CommandServiceConfig DefaultCommandConfig
        {
            get
            {
                return new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Info,
                    ThrowOnError = true
                };
            }
        }
    }
}
