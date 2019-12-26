using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // the class that handles connecting to discord
    // First connect, then add typereaders, then add modules, then set status.
    public class OriNetworkService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;
        //private readonly LogService _console;
        private readonly IConfigurationRoot _config;

        public OriNetworkService(DiscordSocketClient client, CommandService commandService,
            IServiceProvider provider, IConfigurationRoot config)
        {
            Console.WriteLine("-- Initializing network services. --");
            _client = client;
            _commandService = commandService;
            _provider = provider;
            _config = config;
        }

        // goes ahead and builds the network service by loading all of the main components.
        public async Task CompileAsync(Dictionary<Type, TypeReader> typeReaders, List<Type> modules, string token = "")
        {
            // change how the token is handled?
            token = string.IsNullOrWhiteSpace(token) ? _config["keys:discord"] : token;

            if (string.IsNullOrWhiteSpace(token))
                throw new NullReferenceException("The token cannot be empty.");

            await _client.LoginAsync(TokenType.Bot, token);
            if (typeReaders.Count > 0)
                foreach (KeyValuePair<Type, TypeReader> pair in typeReaders)
                {
                    _commandService.AddTypeReader(pair.Key, pair.Value);
                    Console.WriteLine($"-- Compiled '{pair.Key.Name}' to Orikivo.TypeReaders. --");
                }
            if (modules.Count > 0)
                foreach (Type moduleType in modules)
                {
                    await _commandService.AddModuleAsync(moduleType, _provider);
                    /* You can export module info here to be stored onto the website */
                    //Console.WriteLine($"-- Compiled '{moduleType.Name}' to Orikivo.Modules. --");
                }
        }

        public async Task StartAsync()
            => await _client.StartAsync();

        public async Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
        {
            await _client.SetGameAsync(name, streamUrl, type);
            //_provider.GetRequiredService<LogService>().Debug("IActivity updated.");
        }

        public async Task SetStatusAsync(UserStatus status)
        {
            await _client.SetStatusAsync(status);
            //_provider.GetRequiredService<LogService>().Debug("UserStatus updated.");
        }

        public async Task StopAsync()
            => await _client.StopAsync();

        public async Task RemoveModuleAsync<TModule>() where TModule : ModuleBase
            => await _commandService.RemoveModuleAsync(typeof(TModule));

    }
}
