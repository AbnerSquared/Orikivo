using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Storage;
using Orikivo.Networking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orikivo.Systems.Dependencies.Entities;
using Orikivo.Logging;
using Orikivo.Wrappers;

namespace Orikivo
{
    public class UnitManager
    {
        // The config.json file integration.
        public IConfigurationRoot Config { get; set; }
        private const string CONFIG_PATH = "config.json";

        public UnitManager(string[] args)
        {
            Config = GetConfigBuilder().Build();
        }

        private static ConfigurationBuilder GetConfigBuilder()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(CONFIG_PATH);
            return builder;
        }

        public static async Task CreateInstanceAsync(string[] args)
        {
            UnitManager instance = new UnitManager(args);
            await instance.CreateInstanceAsync();
        }

        public async Task CreateInstanceAsync()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.AddSingletons(this);
            
            ServiceProvider provider = collection.BuildServiceProvider();
            await provider.GetRequiredService<NetworkManager>().ConnectAsync();
            await Task.Delay(-1);
        }

        public void AddSingletonsToService(IServiceCollection collection)
        {
            Global.Client = new DiscordSocketClient(SocketEntity.GetDefault());
            Global.WebClient = OriWebClient.Default;

            LockedDblWrapper dblWrapper = new LockedDblWrapper(Global.Client, Config["tokens:dbl"]);

            collection
                .AddSingleton<DataContainer>() // Where all data is stored.
                .AddSingleton(new DiscordSocketClient(SocketEntityW.GetDefault())) // Orikivo's required boot.
                .AddSingleton(new CommandService(CommandEntity.GetDefault())) // Orikivo's command information.
                .AddSingleton<NetworkManager>() // Controls Discord connection.
                .AddSingleton<EventLogger>() // Controls events that occur on Orikivo.
                .AddSingleton(Config); // All of the passwords for public services.
        }
    }
}