using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orikivo.Systems.Dependencies;
using Orikivo.Systems.Dependencies.Entities;
using Orikivo.Systems.Presets;
using Orikivo.Systems.Services;
using Orikivo.Storage;
using Orikivo;
using Orikivo.Dynamic;
using Orikivo.Networking;
using Orikivo.Wrappers;
using Orikivo.Logging;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Orikivo.Systems
{
    public class UnitCollection
    {
        // public DiscordSocketClient GlobalClient; need a static
        // DataContainer, and a static GlobalClient
        public IConfigurationRoot Configuration { get; }

        public UnitCollection(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("configuration.json");
            Configuration = configBuilder.Build();
        }

        public static async Task CollectServiceAsync(string[] args)
        {
            var start = new UnitCollection(args);
            await start.CollectServiceAsync();
        }

        public async Task CollectServiceAsync()
        {
            var services = new ServiceCollection();

            BuildServices(services);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<ConsoleDependency>();
            //provider.GetRequiredService<EventDependency>();
            provider.GetRequiredService<EventLogger>();
            await provider.GetRequiredService<NetworkManager>().ConnectAsync();

            ReadAssembly(Assembly.GetEntryAssembly());

            await Task.Delay(-1);
        }

        public static void ReadAssembly(Assembly a)
        {
            StringBuilder sb = new StringBuilder();

            a.ExportedTypes.OrderBy(x => x.ToString()).ForEach(x => { sb.AppendLine(x.ToString()); x.Debug("export name"); });
            Manager.WriteTextAsync(sb.ToString(), ".//misc//tree.txt");
        }

        private void BuildServices(IServiceCollection services)
        {
            Global.Client = new DiscordSocketClient(SocketEntity.GetDefault());
            Global.WebClient = OriWebClient.Default;

            services
                .AddSingleton<DataContainer>()
                //.AddSingleton(new LockedDblWrapper())
                //.AddSingleton(new DiscordShardedClient(new int[] {0}, SocketEntity.GetDefault()))
                .AddSingleton(new DiscordSocketClient(SocketEntity.GetDefault()))
                .AddSingleton(new CommandService(CommandEntity.GetDefault()))
                .AddSingleton(new AudioDependency())
                .AddSingleton(new CancellationTokenSource())
                .AddSingleton<DynamicManager>()
                //.AddSingleton<EventDependency>()
                .AddSingleton<NetworkManager>()
                .AddSingleton<ConsoleDependency>()
                .AddSingleton<AudioService>()
                .AddSingleton<InsultService>()
                .AddSingleton<StatusService>()
                .AddSingleton<GuildPrefUtility>()
                .AddSingleton<Exceptions>()
                .AddSingleton<EventLogger>()
                .AddSingleton<Random>()
                .AddSingleton(Configuration);
        }
    }
}
