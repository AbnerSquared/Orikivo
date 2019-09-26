using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Wrappers;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Orikivo.Networking
{
    public class NetworkManager
    {
        // Use the Global class to load in statuses..
        private DiscordSocketClient _client;
        private CommandService _service;
        private IConfigurationRoot _config;
        private IServiceProvider _provider;
        private DataContainer _data;

        public NetworkManager(DiscordSocketClient client, CommandService service,
            IConfigurationRoot config, IServiceProvider provider, DataContainer data)
        {
            _client = client;
            _service = service;
            _config = config;
            _provider = provider;
            _data = data;
        }

        public async Task ConnectAsync()
        {
            string token = _config["api:discord"];

            if (string.IsNullOrEmpty(token))
                throw new Exception("The token specified is invalid.");

            await _client.LoginAsync(TokenType.Bot, token);
            await StartAsync();
        }

        public async Task StartAsync()
        {
            // set global properties...
            Global.Client = _client;
            Global.Service = _service;
            //Global2.Data = _data;

            await _client.StartAsync();
            AddTypeReaders();

            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            _data.EnsureList(_service.Modules);
            
            await SetGameAsync(_data.Global.Activity);
            //ResourceContainer.ReadAssembly(Assembly.GetEntryAssembly());
        }

        public async Task SetGameAsync(CompactActivity activity)
        {
            await _client.SetGameAsync(activity.Name, type: activity.Type);
        }

        public async Task StopAsync()
        {
            await _client.StopAsync();
        }

        public void AddTypeReaders()
        {
            _service.AddTypeReader<Emoji>(new EmojiTypeReader());
            _service.AddTypeReader<AccountOption>(new AccountOptionTypeReader());
            _service.AddTypeReader<object>(new ObjectTypeReader());
            _service.AddTypeReader<WidgetType>(new WidgetTypeTypeReader());
        }
    }
}