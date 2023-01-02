using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo.Framework
{
    // TODO: Make a generic interface BaseConnectionService with the same methods to make it easier
    public class InteractionConnectionService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _service;
        private readonly IServiceProvider _provider;
        private readonly IConfigurationRoot _config;
        private bool _compiled;

        public InteractionConnectionService(DiscordSocketClient client, InteractionService service,
            IServiceProvider provider, IConfigurationRoot config)
        {
            _client = client;
            _service = service;
            _provider = provider;
            _config = config;
        }

        internal async Task CompileAsync(Dictionary<Type, TypeReader> typeReaders, List<Type> modules, Dictionary<Type, TypeConverter> typeConverters)
        {
            if (_compiled)
                return;

            if (string.IsNullOrWhiteSpace(_config["token"]))
                throw new NullReferenceException("A token is required in order to connect. This can be configured in config.json, with the name 'token'.");

            await _client.LoginAsync(TokenType.Bot, _config["token"]);

            if (typeConverters?.Count > 0)
                foreach ((Type type, TypeConverter converter) in typeConverters)
                {
                    _service.AddTypeConverter(type, converter);
                    Logger.Debug($"Compiled '{type.Name}' as a type converter");
                }

            if (typeReaders?.Count > 0)
                foreach ((Type type, TypeReader reader) in typeReaders)
                {
                    _service.AddTypeReader(type, reader);
                    Logger.Debug($"Compiled '{type.Name}' as a typereader");
                }

            if (modules?.Count > 0)
                foreach (Type moduleType in modules)
                {
                    await _service.AddModuleAsync(moduleType, _provider);
                    Logger.Debug($"Compiled '{moduleType.Name}' as a module");
                }
            _compiled = true;
        }

        /// <summary>
        /// Starts the connection between Discord and the <see cref="InteractionClient"/>.
        /// </summary>
        public async Task StartAsync()
            => await _client.StartAsync();

        /// <summary>
        /// Sets the current status for the <see cref="InteractionClient"/>.
        /// </summary>
        public async Task SetStatusAsync(UserStatus status)
            => await _client.SetStatusAsync(status);

        /// <summary>
        /// Sets the activity of the <see cref="Client"/>.
        /// </summary>
        public async Task SetActivityAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
            => await _client.SetGameAsync(name, streamUrl, type);

        /// <summary>
        /// Sets the activity of the <see cref="InteractionClient"/>.
        /// </summary>
        public async Task SetActivityAsync(ActivityConfig activity)
        {
            if (activity != null)
                await SetActivityAsync(activity.Name, activity.StreamUrl, activity.Type);
        }

        /// <summary>
        /// Stops the connection to Discord for the <see cref="InteractionClient"/>.
        /// </summary>
        public async Task StopAsync()
            => await _client.StopAsync();
    }
}
