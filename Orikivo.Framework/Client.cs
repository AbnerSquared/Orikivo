using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Find a way to create a virtual StartAsync() to prevent the requirements of DiscordNetworkService and LogService.

    /// <summary>
    /// Represents the central process manager for a Discord bot.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly Dictionary<Type, TypeReader> _typeReaders;
        private readonly List<Type> _modules;
        private readonly ConsoleConfig _consoleConfig;
        private readonly LogConfig _logConfig;

        internal Client(IConfigurationRoot config, IServiceProvider provider, Dictionary<Type, TypeReader> typeReaders,
            List<Type> modules, ConsoleConfig consoleConfig, LogConfig loggerConfig)
        {
            Config = config;
            Provider = provider;
            _typeReaders = typeReaders;
            _modules = modules;
            _consoleConfig = consoleConfig;
            _logConfig = loggerConfig;
        }

        /// <summary>
        /// Defines the configuration set for the <see cref="Client"/>.
        /// </summary>
        public IConfigurationRoot Config { get; }

        /// <summary>
        /// Defines the global <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider Provider { get; }

        private DiscordNetworkService Network => Provider.GetRequiredService<DiscordNetworkService>();
        private LogService Logger => Provider.GetRequiredService<LogService>();

        // callback is the action to execute.
        /// <summary>
        /// Initializes the connection between Discord and the <see cref="Client"/>. Once this starts, methods executed outside of this process will be ignored.
        /// </summary>
        /// <param name="prelaunch">The action to execute before the <see cref="Client"/> starts.</param>
        public async Task StartAsync(Action<Client> prelaunch, CancellationToken cancelToken)
        {
            if (_consoleConfig != null)
                Logger.ConsoleConfig = _consoleConfig;

            if (_logConfig != null)
                Logger.LogConfig = _logConfig;

            prelaunch.Invoke(this);

            await Network.CompileAsync(_typeReaders, _modules);
            await Network.StartAsync();
            await Task.Delay(-1, cancelToken);
        }

        /// <summary>
        /// Stops the connection between Discord and the <see cref="Client"/>.
        /// </summary>
        public async Task StopAsync()
        {
            //if (_cancelSource)
            await Network.StopAsync();
        }

        /// <summary>
        /// Sets the current status for the <see cref="Client"/>. If <see cref="StartAsync"/> was already called, this method will execute once that process ends.
        /// </summary>
        public async Task SetStatusAsync(UserStatus status)
            => await Network.SetStatusAsync(status);

        /// <summary>
        /// Sets the game for the <see cref="Client"/>. If <see cref="StartAsync"/> was already called, this method will execute once that process ends.
        /// </summary>
        public async Task SetGameAsync(string name, string streamUrl = null, ActivityType activity = ActivityType.Playing)
            => await Network.SetGameAsync(name, streamUrl, activity);

        // TODO: Figure out if this is even needed
        public void Dispose() { }
    }
}
