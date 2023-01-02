using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo.Framework
{
    public class InteractionClient
    {
        private readonly Dictionary<Type, TypeReader> _typeReaders;
        private readonly Dictionary<Type, TypeConverter> _typeConverters;
        private readonly List<Type> _modules;

        internal InteractionClient(IServiceProvider provider, Dictionary<Type, TypeReader> typeReaders,
            List<Type> modules, Dictionary<Type, TypeConverter> typeConverters)
        {
            Provider = provider;
            _typeReaders = typeReaders;
            _modules = modules;
            _typeConverters = typeConverters;
        }

        /// <summary>
        /// Represents the collection of referenced services from a <see cref="ClientBuilder"/>.
        /// </summary>
        public IServiceProvider Provider { get; }

        public UserStatus Status { get; set; } = UserStatus.Online;

        public ActivityConfig Activity { get; set; }

        private InteractionConnectionService Network => Provider.GetRequiredService<InteractionConnectionService>();

        public InteractionClient WithActivity(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
        {
            Activity = new ActivityConfig { Name = name, StreamUrl = streamUrl, Type = type };
            return this;
        }

        public InteractionClient WithActivity(ActivityConfig activity)
        {
            Activity = activity;
            return this;
        }

        public InteractionClient WithStatus(UserStatus status)
        {
            Status = status;
            return this;
        }

        /// <summary>
        /// Initializes the connection between Discord and the <see cref="Client"/>. Once this starts, methods executed outside of this process will be executed once this Task ends.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Network.CompileAsync(_typeReaders, _modules, _typeConverters);
            await Network.SetStatusAsync(Status);
            await Network.SetActivityAsync(Activity);
            await Network.StartAsync();
            await Task.Delay(-1, cancellationToken);
        }
    }
}
