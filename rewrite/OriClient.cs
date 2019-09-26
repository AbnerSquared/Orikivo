using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // the core of orikivo
    // all services, config data, resources, and everything else is contained in this class.
    // TODO: Create a set of options for the Console to use; (window size, console title, console colors, etc...)
    public class OriClient : IDisposable
    {
        public OriClient(IConfigurationRoot config, IServiceProvider provider, Dictionary<Type, TypeReader> typeReaders, List<Type> modules, OriConsoleConfig consoleConfig, OriLoggerConfig loggerConfig)
        {
            Config = config;
            Provider = provider;
            TypeReaders = typeReaders;
            Modules = modules;
            //Network = Provider.GetRequiredService<OriNetworkService>();
            ConsoleConfig = consoleConfig;
            LoggerConfig = loggerConfig;
        }

        public IConfigurationRoot Config { get; }
        public IServiceProvider Provider { get; }
        //private OriNetworkService Network { get; }
        private Dictionary<Type, TypeReader> TypeReaders { get; }
        private List<Type> Modules { get; }
        private OriConsoleConfig ConsoleConfig { get; }
        private OriLoggerConfig LoggerConfig { get; }

        public async Task StartAsync(string token = "")
        {
            Console.WriteLine("-- Now launching Orikivo. --");
            OriLoggerService Logger = Provider.GetRequiredService<OriLoggerService>();
            Logger.ConsoleConfig = ConsoleConfig;
            Logger.LoggerConfig = LoggerConfig;

            Provider.GetRequiredService<OriEventHandler>();
            await Provider.GetRequiredService<OriNetworkService>().CompileAsync(TypeReaders, Modules, token);
            await Provider.GetRequiredService<OriNetworkService>().StartAsync();
            await Task.Delay(-1); // once this starts, calling from OriClient no longer works.
        }

        public async Task StopAsync()
        {
            await Provider.GetRequiredService<OriNetworkService>().StopAsync();
        }
        public async Task SetStatusAsync(UserStatus status)
        {
            Console.WriteLine("[Debug] -- Overwriting UserStatus. --");
            await Provider.GetRequiredService<OriNetworkService>().SetStatusAsync(status);
        }
        public async Task SetGameAsync(string name, string streamUrl = null, ActivityType activity = ActivityType.Playing)
        {
            Console.WriteLine("[Debug] -- Overwriting IActivity. --");
            await Provider.GetRequiredService<OriNetworkService>().SetGameAsync(name, streamUrl, activity);
        }

        // releases all resources used up by this class.
        public void Dispose()
        {
            // what resources are used up?
        }
    }
}
