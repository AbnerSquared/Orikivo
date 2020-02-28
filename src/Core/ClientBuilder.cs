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
        /// <summary>
        /// Inserts all of the default services for the base <see cref="Client"/> to the specified <see cref="ServiceCollection"/>.
        /// </summary>
        public static void SetDefaultServices(ServiceCollection services)
        {
            services
                .AddSingleton<DiscordNetworkService>() // Handles the connection to Discord.
                .AddSingleton<LogService>() // Handles all logging for Orikivo.
                .AddSingleton<OriJsonContainer>() // A data container that is passed along all inheriting classes.
                .AddSingleton<EventHandler>() // Manages all events that occur from the Discord API.
                .AddSingleton<CommandHandler>() // Manages all messages to attempt to parse as a valid command.
                // TODO: Create a message handler that can create internal handles for separate users.
                .AddSingleton<GameManager>();
        }

        /// <summary>
        /// Initializes a new default <see cref="ClientBuilder"/>.
        /// </summary>
        /// <param name="basePath">The base directory at which the configuration will be stored. If unspecified, it will default to <see cref="Directory.GetCurrentDirectory"/>.</param>
        public ClientBuilder(string basePath = null)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Check.NotNull(basePath) ? basePath : Directory.GetCurrentDirectory())
                .AddJsonFile("config.json").Build();
            Services = new ServiceCollection();

            // These services are required with every Discord bot and is always implemented.
            Services
                .AddSingleton(new DiscordSocketClient(DiscordConfig.DefaultSocketConfig))
                .AddSingleton(new CommandService(DiscordConfig.DefaultServiceConfig))
                .AddSingleton(Config);
        }

        /// <summary>
        /// Represents the configuration specified.
        /// </summary>
        public IConfigurationRoot Config { get; }

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
        /// Gets or sets the <see cref="Orikivo.ConsoleConfig"/> to set for the <see cref="Console"/>.
        /// </summary>
        public ConsoleConfig ConsoleConfig { get; set; }

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
        {
            if (!TypeReaders.TryAdd(type, reader))
                if (TypeReaders[type] != reader)
                    TypeReaders[type] = reader;
        }

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
        /// Adds the specified module.
        /// </summary>
        public void AddModule<T>() where T : class
        {
            if (!Modules.Contains(typeof(T)))
                Modules.Add(typeof(T));
        }

        /// <summary>
        /// Removes the specified module.
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
        /// Compiles the <see cref="ClientBuilder"/> and initializes a new <see cref="Client"/>.
        /// </summary>
        public Client Build()
            => new Client(Config, Services.BuildServiceProvider(), TypeReaders, Modules, ConsoleConfig, LogConfig);
    }
}
