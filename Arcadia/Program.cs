using System.Collections.Generic;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Orikivo.Drawing;
using Orikivo.Framework;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Casino;
using Arcadia.Graphics;
using Arcadia.Modules;
using Arcadia.Multiplayer;
using Arcadia.Multiplayer.Games;
using Arcadia.Services;
using Orikivo;
using Orikivo.Text;

namespace Arcadia
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Logger.DebugAllowed = false;
#if DEBUG
                Logger.DebugAllowed = true;
#endif
                var cancelSource = new CancellationTokenSource();

                var layout = new ConsoleLayout
                {
                    Title = $"Orikivo Arcade: {ArcadeData.Version}",
                    BackgroundColor = null,
                    ForegroundColor = null,
                    CursorVisible = false
                };

                layout.Set();

                var builder = new ClientBuilder();

                builder.Services
                    .AddSingleton<GameManager>()
                    .AddSingleton<LocaleProvider>()
                    .AddSingleton<InfoFormatter>()
                    .AddSingleton<InfoService>()
                    .AddSingleton<ArcadeContainer>()
                    .AddSingleton<LogService>()
                    .AddSingleton<CommandHandler>()
                    .AddSingleton<EventHandler>()
                    .AddSingleton<CasinoService>();

                builder.SocketConfig = Orikivo.DiscordConfig.DefaultSocketConfig;
                builder.CommandConfig = Orikivo.DiscordConfig.DefaultCommandConfig;

                builder
                    .AddTypeReader<Item>(new ItemTypeReader())
                    .AddTypeReader<Merit>(new MeritTypeReader())
                    .AddTypeReader<Recipe>(new RecipeTypeReader())
                    .AddTypeReader<Quest>(new QuestTypeReader())
                    .AddTypeReader<Shop>(new ShopTypeReader())
                    .AddTypeReader<ArcadeUser>(new ArcadeUserTypeReader())
                    .AddTypeReader<Wager>(new WagerTypeReader())
                    .AddEnumTypeReader<Casing>()
                    .AddEnumTypeReader<FontType>()
                    .AddEnumTypeReader<PaletteType>()
                    .AddEnumTypeReader<BorderAllow>()
                    .AddEnumTypeReader<ImageScale>()
                    .AddEnumTypeReader<CardGroup>()
                    .AddEnumTypeReader<Gamma>()
                    .AddEnumTypeReader<BorderEdge>()
                    .AddEnumTypeReader<LeaderboardQuery>()
                    .AddEnumTypeReader<LeaderboardSort>()
                    .AddEnumTypeReader<DoublerWinMethod>()
                    .AddEnumTypeReader<StackTraceMode>()
                    .AddEnumTypeReader<ChessOwner>()
                    .AddEnumTypeReader<Privacy>()
                    .AddEnumTypeReader<LayoutType>()
                    .AddEnumTypeReader<RouletteBetMode>();

                builder
                    .AddModule<Core>()
                    .AddModule<Modules.Casino>()
                    .AddModule<Modules.Multiplayer>()
                    .AddModule<Common>()
                    .AddModule<Economy>()
                    .AddModule<Records>();

                Client client = builder.Build();
                client.Status = UserStatus.Online;
                client.Activity = new ActivityConfig
                {
                    Name = "your requests",
                    Type = ActivityType.Listening
                };

                client.Provider.GetRequiredService<EventHandler>();
                client.Provider.GetRequiredService<CommandHandler>();

                client.Provider.GetRequiredService<GameManager>().DefaultGameId = "trivia";
                // TODO: Implement GameInfo instead, where a custom ID can be written to support custom games
                client.Provider.GetRequiredService<GameManager>().Games = new Dictionary<string, GameInfo>
                {
                    ["trivia"] = new GameInfo(new TriviaGame()),
                    ["werewolf"] = new GameInfo(new WerewolfGame()),
                    ["chess"] = new GameInfo(new ChessGame()),
                    ["ultimatetic"] = new GameInfo(new UltimateTicGame())
                };

                await client.StartAsync(cancelSource.Token);
            }).GetAwaiter().GetResult();
        }
    }
}
