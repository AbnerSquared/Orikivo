using Microsoft.Extensions.DependencyInjection;
using Orikivo.Framework;

namespace Orikivo
{
    public static class ClientExtensions
    {
        /// <summary>
        /// Ensures that the <see cref="IServiceProvider"/> contains the required services for the default <see cref="Client"/>.
        /// </summary>
        public static void EnsureDefaultServices(this Client client)
        {
            client.Provider.GetRequiredService<EventHandler>();
            client.Provider.GetRequiredService<CommandHandler>();
        }

        /// <summary>
        /// Inserts all of the default services for the base <see cref="Client"/> to the specified <see cref="ServiceCollection"/>.
        /// </summary>
        public static ClientBuilder SetDefaultServices(this ClientBuilder builder)
        {
            builder.Services
                .AddSingleton<LogService>() // Handles all logging for Orikivo.
                .AddSingleton<OriJsonContainer>() // A data container that is passed along all inheriting classes.
                .AddSingleton<EventHandler>() // Manages all events that occur from the Discord API.
                .AddSingleton<CommandHandler>();

            builder.SocketConfig = DiscordConfig.DefaultSocketConfig;
            builder.CommandConfig = DiscordConfig.DefaultCommandConfig;

            return builder;
        }
    }
}
