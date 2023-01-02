using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Orikivo.Framework
{
    public class InteractionClientBuilder
    {
        private const string CONFIG_FILE_NAME = "config.json";

        /// <summary>
        /// Initializes a new default <see cref="ClientBuilder"/>.
        /// </summary>
        /// <param name="configPath">The base directory at which the configuration will be stored. If unspecified, it will default to <see cref="Directory.GetCurrentDirectory"/>.</param>
        public InteractionClientBuilder(string configPath = null)
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

        public Dictionary<Type, TypeReader> TypeReaders { get; set; } = new Dictionary<Type, TypeReader>();

        public Dictionary<Type, TypeConverter> TypeConverters { get; set; } = new Dictionary<Type, TypeConverter>();

        public List<Type> Modules { get; set; } = new List<Type>();

        public InteractionServiceConfig InteractionConfig { get; set; }

        public DiscordSocketConfig SocketConfig { get; set; }

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

        public InteractionClientBuilder AddTypeConverter<T>(TypeConverter converter)
            => AddTypeConverter(typeof(T), converter);

        public InteractionClientBuilder AddTypeConverter(Type type, TypeConverter converter)
        {
            if (!TypeConverters.TryAdd(type, converter))
                TypeConverters[type] = converter;

            return this;
        }

        public void RemoveTypeConverter<T>()
            => RemoveTypeConverter(typeof(T));

        public void RemoveTypeConverter(Type type)
        {
            if (TypeConverters.ContainsKey(type))
                TypeConverters.Remove(type);
        }

        public InteractionClientBuilder AddTypeReader<T>(TypeReader reader)
            => AddTypeReader(typeof(T), reader);

        public InteractionClientBuilder AddTypeReader(Type type, TypeReader reader)
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

        public InteractionClientBuilder AddModule<T>()
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

        public InteractionClient Build()
        {
            var client = new DiscordSocketClient(SocketConfig);

            Services
                .AddSingleton(client)
                .AddSingleton(new InteractionService(client, InteractionConfig))
                .AddSingleton(Config.AddJsonFile(!string.IsNullOrWhiteSpace(_configPath) ? _configPath : CONFIG_FILE_NAME).Build())
                .AddSingleton<InteractionConnectionService>();

            return new InteractionClient(Services.BuildServiceProvider(), TypeReaders, Modules, TypeConverters);
        }
    }
}
