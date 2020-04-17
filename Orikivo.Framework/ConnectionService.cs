using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo.Framework
{
    public class ConnectionService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;
        private readonly IConfigurationRoot _config;
        private bool _compiled;

        public ConnectionService(DiscordSocketClient client, CommandService commandService,
            IServiceProvider provider, IConfigurationRoot config)
        {
            _client = client;
            _commandService = commandService;
            _provider = provider;
            _config = config;
        }

        internal async Task CompileAsync(Dictionary<Type, TypeReader> typeReaders, List<Type> modules)
        {
            if (_compiled)
                return;

            if (string.IsNullOrWhiteSpace(_config["token"]))
                throw new NullReferenceException("A specified token is required in order to connect. This can be configured in config.json, at 'keys:discord'.");

            await _client.LoginAsync(TokenType.Bot, _config["token"]);

            if (typeReaders?.Count > 0)
                foreach ((Type type, TypeReader reader) in typeReaders)
                {
                    _commandService.AddTypeReader(type, reader);
                }

            if (modules?.Count > 0)
                foreach (Type moduleType in modules)
                {
                    await _commandService.AddModuleAsync(moduleType, _provider);
                }

            _compiled = true;
        }

        /// <summary>
        /// Starts the connection between Discord and the <see cref="Client"/>.
        /// </summary>
        public async Task StartAsync()
        {
            await _client.StartAsync();
        }

        public async Task SetStatusAsync(StatusConfig config)
        {
            await SetStatusAsync(config.Status);
            if (config.Activity != null)
                await SetGameAsync(config.Activity.Name, config.Activity.StreamUrl, config.Activity.Type);
        }

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
        public async Task RemoveModuleAsync<TModule>()
            where TModule : class
            => await _commandService.RemoveModuleAsync(typeof(TModule));

        /// <summary>
        /// Removes the specified command module.
        /// </summary>
        public async Task RemoveModuleAsync(Type moduleType)
            => await _commandService.RemoveModuleAsync(moduleType);
    }
}
