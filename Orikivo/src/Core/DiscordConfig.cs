using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace Orikivo
{
    public static class DiscordConfig
    {
        public static DiscordSocketConfig DefaultSocketConfig
            => new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100,
                LargeThreshold = 250,
                GatewayIntents = GatewayIntents.Guilds
                // Create a UserTypeReader that directly attempts to load a user
                // Caching users is disabled with this intent.
                                     | GatewayIntents.GuildMembers
                                     | GatewayIntents.GuildMessages
                                     | GatewayIntents.GuildMessageReactions
                                     | GatewayIntents.DirectMessages
                                     | GatewayIntents.DirectMessageReactions
            };

        public static InteractionServiceConfig DefaultInteractionConfig
            => new InteractionServiceConfig
            {
                UseCompiledLambda = true,
                DefaultRunMode = Discord.Interactions.RunMode.Async,
                LogLevel = LogSeverity.Info,
                ThrowOnError = true
            };

        public static CommandServiceConfig DefaultCommandConfig
            => new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = Discord.Commands.RunMode.Async,
                LogLevel = LogSeverity.Info,
                ThrowOnError = true
            };
    }
}
