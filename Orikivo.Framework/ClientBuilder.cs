using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Orikivo.Framework
{
    // TODO: Learn how to shard.
    /// <summary>
    /// Represents a constructor for an <see cref="Client"/>.
    /// </summary>
    public class ClientBuilder
    {
        private const string CONFIG_FILE_NAME = "config.json";

        /// <summary>
        /// Initializes a new default <see cref="ClientBuilder"/>.
        /// </summary>
        /// <param name="configPath">The base directory at which the configuration will be stored. If unspecified, it will default to <see cref="Directory.GetCurrentDirectory"/>.</param>
        public ClientBuilder(string configPath = null)
        {
            Config = new ConfigurationBuilder();

            if (!string.IsNullOrWhiteSpace(configPath))
                SetConfigPath(configPath);

            Services = new ServiceCollection();
        }

        private string _configPath;

        private IConfigurationBuilder Config { get; }

        /// <summary>
        /// Represents the collection of services that can be referenced in a <see cref="Client"/>.
        /// </summary>
        public ServiceCollection Services { get; }

        public Dictionary<Type, Discord.Commands.TypeReader> TypeReaders { get; set; } = new Dictionary<Type, Discord.Commands.TypeReader>();

        public Dictionary<Type, Discord.Interactions.TypeReader> InteractionTypeReaders { get; set; } = new Dictionary<Type, Discord.Interactions.TypeReader>();

        public List<Type> Modules { get; set; } = new List<Type>();

        public InteractionServiceConfig InteractionConfig { get; set; }

        public DiscordSocketConfig SocketConfig { get; set; }

        public CommandServiceConfig CommandConfig { get; set; }

        public void SetConfigPath(string path)
        {
            string directory = Path.GetDirectoryName(path);
            _configPath = Path.GetFileName(path);
            Config.SetBasePath(directory);
        }

        public void SetDefaultConfigPath()
        {
            string directory = Directory.GetCurrentDirectory();
            _configPath = null;
            Config.SetBasePath(directory);
        }

        public ClientBuilder AddTypeReader<T>(Discord.Commands.TypeReader reader)
            => AddTypeReader(typeof(T), reader);

        public ClientBuilder AddTypeReader(Type type, Discord.Commands.TypeReader reader)
        {
            if (!TypeReaders.TryAdd(type, reader))
                TypeReaders[type] = reader;

            return this;
        }

        public void RemoveTypeReader<T>()
            => RemoveTypeReader(typeof(T));

        public void RemoveTypeReader(Type type)
        {
            if (TypeReaders.ContainsKey(type))
                TypeReaders.Remove(type);
        }

        public ClientBuilder AddModule<T>()
            where T : class
        {
            if (!Modules.Contains(typeof(T)))
                Modules.Add(typeof(T));

            return this;
        }

        public void RemoveModule<T>()
            where T : class
        {
            if (Modules.Contains(typeof(T)))
                Modules.Remove(typeof(T));
        }

        public Client Build()
        {
            Services
                .AddSingleton(new DiscordSocketClient(SocketConfig))
                .AddSingleton(new CommandService(CommandConfig))
                .AddSingleton(Config.AddJsonFile(!string.IsNullOrWhiteSpace(_configPath) ? _configPath : CONFIG_FILE_NAME).Build())
                .AddSingleton<ConnectionService>();

            return new Client(Services.BuildServiceProvider(), TypeReaders, Modules);
        }
    }
}
