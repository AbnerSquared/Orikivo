using Discord;
using Microsoft.Extensions.DependencyInjection;
using Orikivo.Drawing;
using Orikivo.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Casino;
using Arcadia.Graphics;
using Arcadia.Modules;
using Arcadia.Services;
using MongoDB.Driver;
using Orikivo;

namespace Arcadia
{
    // Entry-point for Orikivo Arcade
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var _cancelSource = new CancellationTokenSource();

                var layout = new ConsoleLayout
                {
                    Title = $"Orikivo Arcade: {Orikivo.OriGlobal.ClientVersion}",
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
                .AddSingleton<CommandHandler>()
                .AddSingleton(new MongoClient());

                builder.SocketConfig = Orikivo.DiscordConfig.DefaultSocketConfig;
                builder.CommandConfig = Orikivo.DiscordConfig.DefaultCommandConfig;

                builder
                    .AddEnumTypeReader<Graphics.CardDeny>()
                    .AddEnumTypeReader<Graphics.Casing>()
                    .AddEnumTypeReader<Graphics.FontType>()
                    .AddEnumTypeReader<Graphics.PaletteType>()
                    .AddEnumTypeReader<BorderAllow>()
                    .AddEnumTypeReader<ImageScale>()
                    .AddEnumTypeReader<CardComponent>()
                    .AddEnumTypeReader<Gamma>()
                    .AddEnumTypeReader<BorderEdge>()
                    .AddEnumTypeReader<LeaderboardQuery>()
                    .AddEnumTypeReader<LeaderboardSort>()
                    .AddEnumTypeReader<TickWinMethod>();

                builder
                    .AddModule<Core>()
                    .AddModule<Modules.Casino>()
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
