using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo.Framework
{
    /// <summary>
    /// Represents the central process manager for a Discord bot.
    /// </summary>
    public class Client
    {
        private readonly Dictionary<Type, TypeReader> _typeReaders;
        private readonly List<Type> _modules;

        internal Client(IConfigurationRoot config, IServiceProvider provider, Dictionary<Type, TypeReader> typeReaders,
            List<Type> modules)
        {
            Config = config;
            Provider = provider;
            _typeReaders = typeReaders;
            _modules = modules;
        }

        /// <summary>
        /// Defines the configuration set for the <see cref="Client"/>.
        /// </summary>
        public IConfigurationRoot Config { get; }

        /// <summary>
        /// Defines the global <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider Provider { get; }

        public StatusConfig Status { get; set; }

        private ConnectionService Network => Provider.GetRequiredService<ConnectionService>();

        /// <summary>
        /// Initializes the connection between Discord and the <see cref="Client"/>. Once this starts, methods executed outside of this process will be ignored.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Network.CompileAsync(_typeReaders, _modules);

            if (Status != null)
                await Network.SetStatusAsync(Status);
    
            await Network.StartAsync();
            await Task.Delay(-1, cancellationToken);
        }
    }
}
