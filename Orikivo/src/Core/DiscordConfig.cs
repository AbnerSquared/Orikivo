using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Orikivo
{
    public static class DiscordConfig
    {
        public static DiscordSocketConfig DefaultSocketConfig
            => new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                LargeThreshold = 250,
                RateLimitPrecision = RateLimitPrecision.Millisecond,
                GatewayIntents = GatewayIntents.Guilds
                                     | GatewayIntents.GuildMessages
                                     | GatewayIntents.GuildMessageReactions
                                     | GatewayIntents.DirectMessages
                                     | GatewayIntents.DirectMessageReactions
            };

        public static CommandServiceConfig DefaultCommandConfig
            => new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info,
                ThrowOnError = true
            };
    }
}
