using Discord;
using Microsoft.Extensions.DependencyInjection;
using Orikivo;
using Orikivo.Drawing;
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
                .AddSingleton<InfoService>()
                .AddSingleton<ArcadeContainer>()
                .AddSingleton<LogService>()
                .AddSingleton<Orikivo.EventHandler>()
                .AddSingleton<CommandHandler>();

                builder.SocketConfig = Orikivo.DiscordConfig.DefaultSocketConfig;
                builder.CommandConfig = Orikivo.DiscordConfig.DefaultCommandConfig;

                builder
                    .AddEnumTypeReader<CardDeny>()
                    .AddEnumTypeReader<Casing>()
                    .AddEnumTypeReader<FontType>()
                    .AddEnumTypeReader<PaletteType>()
                    .AddEnumTypeReader<BorderAllow>()
                    .AddEnumTypeReader<ImageScale>()
                    .AddEnumTypeReader<CardComponent>()
                    .AddEnumTypeReader<Gamma>()
                    .AddEnumTypeReader<BorderEdge>()
                    .AddEnumTypeReader<LeaderboardFlag>()
                    .AddEnumTypeReader<LeaderboardSort>()
                    .AddEnumTypeReader<TickWinMethod>();

                builder
                    .AddModule<Core>()
                    .AddModule<Casino>()
                    .AddModule<Multiplayer>()
                    .AddModule<Common>();

                Client client = builder.Build();
                client.Status = UserStatus.Online;
                client.Activity = new ActivityConfig
                {
                    Name = "your requests",
                    Type = ActivityType.Listening
                };

                client.Provider.GetRequiredService<Orikivo.EventHandler>();
                client.Provider.GetRequiredService<CommandHandler>();

                await client.StartAsync(_cancelSource.Token);
            }).GetAwaiter().GetResult();
        }
    }
}
