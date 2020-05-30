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
                throw new NullReferenceException("A token is required in order to connect. This can be configured in config.json, with the name 'token'.");

            await _client.LoginAsync(TokenType.Bot, _config["token"]);

            if (typeReaders?.Count > 0)
                foreach ((Type type, TypeReader reader) in typeReaders)
                {
                    _commandService.AddTypeReader(type, reader);
                    Logger.Debug($"Compiled '{type.Name}' as a typereader");
                }

            if (modules?.Count > 0)
                foreach (Type moduleType in modules)
                {
                    await _commandService.AddModuleAsync(moduleType, _provider);
                    Logger.Debug($"Compiled '{moduleType.Name}' as a module");
                }
            _compiled = true;
        }

        /// <summary>
        /// Starts the connection between Discord and the <see cref="Client"/>.
        /// </summary>
        public async Task StartAsync()
            => await _client.StartAsync();

        /// <summary>
        /// Sets the current status for the <see cref="Client"/>.
        /// </summary>
        public async Task SetStatusAsync(UserStatus status)
            => await _client.SetStatusAsync(status);

        /// <summary>
        /// Sets the activity of the <see cref="Client"/>.
        /// </summary>
        public async Task SetActivityAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
            => await _client.SetGameAsync(name, streamUrl, type);

        /// <summary>
        /// Sets the activity of the <see cref="Client"/>.
        /// </summary>
        public async Task SetActivityAsync(ActivityConfig activity)
        {
            if (activity != null)
                await SetActivityAsync(activity.Name, activity.StreamUrl, activity.Type);
        }
        
        /// <summary>
        /// Stops the connection to Discord for the <see cref="Client"/>.
        /// </summary>
        public async Task StopAsync()
            => await _client.StopAsync();
    }
}
