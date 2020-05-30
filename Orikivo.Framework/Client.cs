using Discord;
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

        internal Client(IServiceProvider provider, Dictionary<Type, TypeReader> typeReaders,
            List<Type> modules)
        {
            Provider = provider;
            _typeReaders = typeReaders;
            _modules = modules;
        }

        /// <summary>
        /// Represents the collection of referenced services from a <see cref="ClientBuilder"/>.
        /// </summary>
        public IServiceProvider Provider { get; }

        public UserStatus Status { get; set; } = UserStatus.Online;

        public ActivityConfig Activity { get; set; }

        private ConnectionService Network => Provider.GetRequiredService<ConnectionService>();

        public Client WithActivity(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
        {
            Activity = new ActivityConfig { Name = name, StreamUrl = streamUrl, Type = type };
            return this;
        }

        public Client WithActivity(ActivityConfig activity)
        {
            Activity = activity;
            return this;
        }

        public Client WithStatus(UserStatus status)
        {
            Status = status;
            return this;
        }

        /// <summary>
        /// Initializes the connection between Discord and the <see cref="Client"/>. Once this starts, methods executed outside of this process will be executed once this Task ends.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Network.CompileAsync(_typeReaders, _modules);
            await Network.SetStatusAsync(Status);
            await Network.SetActivityAsync(Activity);
            await Network.StartAsync();
            await Task.Delay(-1, cancellationToken);
        }
    }
}
