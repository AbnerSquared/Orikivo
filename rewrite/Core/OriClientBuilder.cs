using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace Orikivo
{
    // TODO: Learn how to shard.

    /// <summary>
    /// A constructor for a client built for Orikivo.
    /// </summary>
    public class OriClientBuilder
    {
        public OriClientBuilder(/*bool useSharding = false*/)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json").Build();
            Services = new ServiceCollection();

            /*if (useSharding)
                Services.AddSingleton(new DiscordShardedClient(DiscordConfig.DefaultSocketConfig));
            else*/

            Services.AddSingleton(new DiscordSocketClient(DiscordConfig.DefaultSocketConfig)); // discord client

            // A list of required classes needed in order for Orikivo to function.
            Services
                .AddSingleton(new CommandService(DiscordConfig.DefaultServiceConfig)) /* The command service used to handle all commands and modules.  */
                .AddSingleton<OriNetworkService>() /* Handles the connection to Discord. */
                //.AddSingleton<LogService>() /* Handles all called events in general. Is connected with DiscordEventHandler. */
                .AddSingleton<DiscordEventHandler>() /* Manages all events that occur from the Discord API. */
                .AddSingleton<OriJsonContainer>() /* A data container that is passed along all inheriting classes. */
                /* TODO: Create a message handler that can create internal handles for separate users. */
                .AddSingleton<GameManager>() /* Handles all of the processes relating to game lobbies and so forth. */
                .AddSingleton(Config); // Root configuration for Orikivo.
        }

        public IConfigurationRoot Config { get; }
        public ServiceCollection Services { get; }
        public Dictionary<Type, TypeReader> TypeReaders { get; set; } = new Dictionary<Type, TypeReader>();
        public List<Type> Modules { get; set; } = new List<Type>();
        public ConsoleConfig ConsoleConfig { get; set; }
        public LogConfig LogConfig { get; set; }

        /// <summary>
        /// Marks an object to be bound with a specified typereader on compile.
        /// </summary>
        public void AddTypeReader<T>(TypeReader reader)
        {
            Type type = typeof(T);
            if (TypeReaders.ContainsKey(type))
                if (TypeReaders.TryGetValue(type, out TypeReader value))
                {
                    if (value != reader)
                    {
                        TypeReaders[type] = reader;
                        Console.WriteLine($"[Debug] -- Updated TypeReader of Type '{typeof(T)}'. --");
                    }
                    else
                        Console.WriteLine($"[Debug] -- Ignoring TypeReader of Type '{typeof(T)}'; Already included. --");
                    return;
                }
            TypeReaders[type] = reader;
            Console.WriteLine($"[Debug] -- Included TypeReader of Type '{typeof(T)}'. --");
        }

        /// <summary>
        /// Removes a typereader from the existing precompile pool.
        /// </summary>
        public void RemoveTypeReader<T>()
        {
            Type type = typeof(T);
            if (TypeReaders.ContainsKey(type))
            {
                TypeReaders.Remove(type);
                Console.WriteLine($"[Debug] -- Excluded TypeReader of Type '{typeof(T)}'. --");
                return;
            }
            Console.WriteLine($"[Debug] -- Ignoring TypeReader of Type '{typeof(T)}'; Already excluded. --");
        }

        public void AddModule<T>() where T : class
        {
            if (Modules.Contains(typeof(T)))
            {
                Console.WriteLine($"[Debug] -- Ignoring Module of Type '{typeof(T)}'; Already included. --");
                return;
            }
            Modules.Add(typeof(T));
            Console.WriteLine($"[Debug] -- Included Module of Type '{typeof(T)}'. --");
        }

        public void RemoveModule<T>() where T : class
        {
            if (Modules.Contains(typeof(T)))
            {
                Modules.Remove(typeof(T));
                Console.WriteLine($"[Debug] -- Excluded Module of Type '{typeof(T)}'. --");
                return;
            }
            Console.WriteLine($"[Debug] -- Ignoring Module of Type '{typeof(T)}'; Already excluded. --");
        }

        public OriClientBuilder WithService<TService>() where TService : class
        {
            OriClientBuilder oriClientBuilder = this;
            oriClientBuilder.Services.AddSingleton<TService>();
            return oriClientBuilder;
        }

        public OriClientBuilder WithService<TService>(TService instance) where TService : class
        {
            OriClientBuilder oriClientBuilder = this;
            oriClientBuilder.Services.AddSingleton(instance);
            return oriClientBuilder;
        }

        /// <summary>
        /// Compiles all of the properties to create a new client.
        /// </summary>
        public OriClient Build()
        {
            return new OriClient(Config, Services.BuildServiceProvider(), TypeReaders, Modules, ConsoleConfig, LogConfig);
        }
    }
}
