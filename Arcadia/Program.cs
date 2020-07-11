using Discord;
using Microsoft.Extensions.DependencyInjection;
using Orikivo;
using Orikivo.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia
{
    // entrypoint for Orikivo Arcade
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var _cancelSource = new CancellationTokenSource();

                var layout = new ConsoleLayout
                {
                    Title = $"Orikivo Arcade: {OriGlobal.ClientVersion}",
                    BackgroundColor = null,
                    ForegroundColor = null,
                    CursorVisible = false
                };

                layout.Set();

                var builder = new ClientBuilder();

                builder.Services
                .AddSingleton<GameManager>()
                .AddSingleton<InfoService>();

                builder.SetDefaultServices();

                builder
                    .AddModule<CoreModule>()
                    .AddModule<Casino>()
                    .AddModule<Multiplayer>();

                Client client = builder.Build();
                client.Status = UserStatus.Online;
                client.Activity = new ActivityConfig { Name = "your requests", Type = ActivityType.Listening };
                client.EnsureDefaultServices();

                await client.StartAsync(_cancelSource.Token);
            }).GetAwaiter().GetResult();
        }
    }
}
