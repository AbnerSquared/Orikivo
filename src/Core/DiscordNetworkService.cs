using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Represents a service that handles a network connection to Discord.
    /// </summary>
    public class DiscordNetworkService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;
        private readonly IConfigurationRoot _config;
        private readonly LogService _console;

        /// <summary>
        /// Initializes a new <see cref="DiscordNetworkService"/>.
        /// </summary>
        public DiscordNetworkService(DiscordSocketClient client, CommandService commandService,
            IServiceProvider provider, IConfigurationRoot config, LogService console)
        {
            _client = client;
            _commandService = commandService;
            _provider = provider;
            _config = config;
            _console = console;
            _console.Debug("Established network service.");
        }

        /// <summary>
        /// Compiles the <see cref="DiscordNetworkService"/> by adding all specified type-readers and modules.
        /// </summary>
        public async Task CompileAsync(Dictionary<Type, TypeReader> typeReaders, List<Type> modules)
        {
            if (!Check.NotNull(_config["keys:discord"]))
                throw new NullReferenceException("A specified token is required in order to connect. This can be configured in config.json, at 'keys:discord'.");

            await _client.LoginAsync(TokenType.Bot, _config["keys:discord"]);

            if (Check.NotNullOrEmpty(typeReaders))
                foreach ((Type type, TypeReader reader) in typeReaders)
                {
                    _commandService.AddTypeReader(type, reader);
                    _console.Debug($"Compiled '{type}' to a TypeReader.");
                }

            if (Check.NotNullOrEmpty(modules))
                foreach (Type moduleType in modules)
                {
                    await _commandService.AddModuleAsync(moduleType, _provider);
                    _console.Debug($"Compiled '{moduleType.Name}' to a Module.");

                    // TODO: Create a file export to retain all module info.
                }
        }

        /// <summary>
        /// Starts the connection between Discord and the <see cref="Client"/>.
        /// </summary>
        public async Task StartAsync()
            => await _client.StartAsync();

        /// <summary>
        /// Sets the game for the <see cref="Client"/>.
        /// </summary>
        public async Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
            => await _client.SetGameAsync(name, streamUrl, type);

        /// <summary>
        /// Sets the current status for the <see cref="Client"/>.
        /// </summary>
        public async Task SetStatusAsync(UserStatus status)
            => await _client.SetStatusAsync(status);
        
        /// <summary>
        /// Stops the connection between Discord and the <see cref="Client"/>.
        /// </summary>
        public async Task StopAsync()
            => await _client.StopAsync();

        /// <summary>
        /// Removes the specified command module.
        /// </summary>
        /// <typeparam name="TModule">The module to remove.</typeparam>
        public async Task RemoveModuleAsync<TModule>() where TModule : class
            => await _commandService.RemoveModuleAsync(typeof(TModule));

        /// <summary>
        /// Removes the specified command module.
        /// </summary>
        public async Task RemoveModuleAsync(Type moduleType)
            => await _commandService.RemoveModuleAsync(moduleType);
    }
}
