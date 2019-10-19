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
    /// <summary>
    /// Represents the entire process of Orikivo.
    /// </summary>
    public class OriClient : IDisposable
    {
        public OriClient(IConfigurationRoot config, IServiceProvider provider, Dictionary<Type, TypeReader> typeReaders, List<Type> modules, OriConsoleConfig consoleConfig, OriLogConfig loggerConfig)
        {
            Config = config;
            Provider = provider;
            TypeReaders = typeReaders;
            Modules = modules;
            ConsoleConfig = consoleConfig;
            LogConfig = loggerConfig;
        }

        public IConfigurationRoot Config { get; }
        public IServiceProvider Provider { get; }
        private Dictionary<Type, TypeReader> TypeReaders { get; }
        private List<Type> Modules { get; }
        private OriConsoleConfig ConsoleConfig { get; }
        private OriLogConfig LogConfig { get; }
        private OriNetworkService Network => Provider.GetRequiredService<OriNetworkService>();
        private OriConsoleService Console => Provider.GetRequiredService<OriConsoleService>();

        public async Task StartAsync(string token = "")
        {
            Console.WriteLine("-- Now launching Orikivo. --");
            Console.ConsoleConfig = ConsoleConfig;
            Console.LoggerConfig = LogConfig;

            Provider.GetRequiredService<DiscordEventHandler>();
            await Network.CompileAsync(TypeReaders, Modules, token);
            await Network.StartAsync();
            await Task.Delay(-1); // once this starts, calling from OriClient no longer works.
        }

        public async Task StopAsync()
            => await Network.StopAsync();

        public async Task SetStatusAsync(UserStatus status)
            => await Network.SetStatusAsync(status);
        public async Task SetGameAsync(string name, string streamUrl = null, ActivityType activity = ActivityType.Playing)
            => await Network.SetGameAsync(name, streamUrl, activity);

        // TODO: Figure out what needs to be disposed
        public void Dispose() { }
    }
}
