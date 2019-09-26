using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // the entry point of Orikivo. this connects all services
    // that only need to be built once into one area,
    // from which can be called with dependency injection
    // required services should already be in the OriClientBuilder by default.

    public class OriClientBuilder
    {
        public OriClientBuilder(bool useSharding = false)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json").Build();
            Services = new ServiceCollection();
            if (useSharding)
                Services.AddSingleton(new DiscordShardedClient(OriDiscordConfig.DefaultSocketConfig));
            else
                Services.AddSingleton(new DiscordSocketClient(OriDiscordConfig.DefaultSocketConfig)); // discord client

            // a list of all classes that can be passed into any class
            // within the namespace
            Services
                .AddSingleton(new CommandService(OriDiscordConfig.DefaultServiceConfig)) // discord command service
                .AddSingleton<OriNetworkService>() // discord network connection
                .AddSingleton<OriLoggerService>() // console logging
                .AddSingleton<OriEventHandler>() // handles discord events
                .AddSingleton<OriJsonContainer>() // json data container
                .AddSingleton<OriMessageInvoker>() // dynamic message invoker
                .AddSingleton<GameManager>() // game manager
                .AddSingleton(Config); // root config for orikivo
            TypeReaders = new Dictionary<Type, TypeReader>();
            Modules = new List<Type>();
        }

        public IConfigurationRoot Config { get; }
        public ServiceCollection Services { get; }
        public Dictionary<Type, TypeReader> TypeReaders { get; set; }
        public List<Type> Modules { get; set; }
        public OriConsoleConfig ConsoleConfig { get; set; }
        public OriLoggerConfig LoggerConfig { get; set; }

        // adds a typeReader for precompile
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

        // removes a typeReader used for precompile
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

        // build the service provider before returning?
        public OriClient Build()
        {
            return new OriClient(Config, Services.BuildServiceProvider(), TypeReaders, Modules, ConsoleConfig, LoggerConfig);
        }
    }
}
