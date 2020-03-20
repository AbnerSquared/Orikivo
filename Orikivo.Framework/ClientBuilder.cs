using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Orikivo
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

            if (Check.NotNull(configPath))
                SetConfigPath(configPath);
                
            Services = new ServiceCollection();

            // These services are required with every Discord bot and is always implemented.
            Services
                .AddSingleton(new DiscordSocketClient(DiscordConfig.DefaultSocketConfig))
                .AddSingleton(new CommandService(DiscordConfig.DefaultServiceConfig))
                .AddSingleton<DiscordNetworkService>()
                .AddSingleton<LogService>();
        }

        /// <summary>
        /// Sets the <see cref="IConfigurationBuilder"/> to the specified directory with a configuration path that points to a JSON file.
        /// </summary>
        public void SetConfigPath(string path)
        {
            string directory = Path.GetDirectoryName(path);
            _configPath = Path.GetFileName(path);
            Config.SetBasePath(directory);
        }

        /// <summary>
        /// Sets the <see cref="IConfigurationBuilder"/> to the current working directory with a default configuration name of "config.json".
        /// </summary>
        public void SetDefaultConfigPath()
        {
            string directory = Directory.GetCurrentDirectory();
            _configPath = null;
            Config.SetBasePath(directory);
        }

        private string _configPath;

        /// <summary>
        /// Represents a configuration builder for the <see cref="Client"/>.
        /// </summary>
        private IConfigurationBuilder Config { get; }

        /// <summary>
        /// Represents the <see cref="ServiceCollection"/> to provide for the <see cref="Client"/>.
        /// </summary>
        public ServiceCollection Services { get; }

        /// <summary>
        /// Represents a collection of types to be established to a <see cref="TypeReader"/>.
        /// </summary>
        public Dictionary<Type, TypeReader> TypeReaders { get; set; } = new Dictionary<Type, TypeReader>();

        /// <summary>
        /// Represents a collection that specifies the modules to add.
        /// </summary>
        public List<Type> Modules { get; set; } = new List<Type>();

        /// <summary>
        /// Gets or sets the <see cref="Orikivo.ConsoleLayout"/> to set for the <see cref="Console"/>.
        /// </summary>
        public ConsoleLayout ConsoleConfig { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Orikivo.LogConfig"/> to set for a <see cref="LogService"/>.
        /// </summary>
        public LogConfig LogConfig { get; set; }

        /// <summary>
        /// Establishes a <see cref="TypeReader"/> for the specified type.
        /// </summary>
        public void AddTypeReader<T>(TypeReader reader)
            => AddTypeReader(typeof(T), reader);

        /// <summary>
        /// Establishes a <see cref="TypeReader"/> for the specified type.
        /// </summary>
        public void AddTypeReader(Type type, TypeReader reader)
            => TypeReaders.AddOrUpdate(type, reader);

        /// <summary>
        /// Removes the <see cref="TypeReader"/> established for the specified type.
        /// </summary>
        public void RemoveTypeReader<T>()
            => RemoveTypeReader(typeof(T));

        /// <summary>
        /// Removes the <see cref="TypeReader"/> established for the specified type.
        /// </summary>
        public void RemoveTypeReader(Type type)
        {
            if (TypeReaders.ContainsKey(type))
                TypeReaders.Remove(type);
        }

        /// <summary>
        /// Adds the specified module from the <see cref="ClientBuilder"/>.
        /// </summary>
        public void AddModule<T>() where T : class
        {
            if (!Modules.Contains(typeof(T)))
                Modules.Add(typeof(T));
        }

        /// <summary>
        /// Removes the specified module from the <see cref="ClientBuilder"/>.
        /// </summary>
        public void RemoveModule<T>() where T : class
        {
            if (Modules.Contains(typeof(T)))
                Modules.Remove(typeof(T));
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <see cref="TService"/> to <see cref="Services"/>.
        /// </summary>
        public ClientBuilder AddSingleton<TService>() where TService : class
        {
            Services.AddSingleton<TService>();
            return this;
        }

        /// <summary>
        /// Adds a singleton service of the type in <see cref="TService"/> with a specified instance to <see cref="Services"/>.
        /// </summary>
        public ClientBuilder AddSingleton<TService>(TService instance) where TService : class
        {
            Services.AddSingleton(instance);
            return this;
        }

        /// <summary>
        /// Adds a transient service of the type specified in <see cref="TService"/> to <see cref="Services"/>.
        /// </summary>
        public ClientBuilder AddTransient<TService>() where TService : class
        {
            Services.AddTransient<TService>();
            return this;
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <see cref="TService"/> to <see cref="Services"/>.
        /// </summary>
        public ClientBuilder AddScoped<TService>() where TService : class
        {
            Services.AddScoped<TService>();
            return this;
        }

        public ClientBuilder AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Services.AddScoped<TService, TImplementation>();
            return this;
        }

        /// <summary>
        /// Compiles the <see cref="ClientBuilder"/> and initializes a new <see cref="Client"/>.
        /// </summary>
        public Client Build()
            => new Client(Config.AddJsonFile(Check.NotNull(_configPath) ? _configPath : CONFIG_FILE_NAME).Build(),
                Services.AddSingleton(Config).BuildServiceProvider(), TypeReaders, Modules, ConsoleConfig, LogConfig);
    }
}
