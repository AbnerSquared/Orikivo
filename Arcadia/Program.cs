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

namespace Arcadia
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Logger.DebugAllowed = true;
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
                    .AddSingleton<InfoService>()
                    .AddSingleton<ArcadeContainer>()
                    .AddSingleton<LogService>()
                    .AddSingleton<EventHandler>()
                    .AddSingleton<CommandHandler>();

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
                    .AddEnumTypeReader<CardDeny>()
                    .AddEnumTypeReader<Casing>()
                    .AddEnumTypeReader<FontType>()
                    .AddEnumTypeReader<PaletteType>()
                    .AddEnumTypeReader<BorderAllow>()
                    .AddEnumTypeReader<ImageScale>()
                    .AddEnumTypeReader<CardComponent>()
                    .AddEnumTypeReader<Gamma>()
                    .AddEnumTypeReader<BorderEdge>()
                    .AddEnumTypeReader<LeaderboardQuery>()
                    .AddEnumTypeReader<LeaderboardSort>()
                    .AddEnumTypeReader<TickWinMethod>()
                    .AddEnumTypeReader<StackTraceMode>()
                    .AddEnumTypeReader<ChessOwner>()
                    .AddEnumTypeReader<RouletteBetMode>();

                builder
                    .AddModule<Core>()
                    .AddModule<Modules.Casino>()
                    .AddModule<Modules.Multiplayer>()
                    .AddModule<Common>();

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
                client.Provider.GetRequiredService<GameManager>().Games = new Dictionary<string, GameBase>
                {
                    ["trivia"] = new TriviaGame(),
                    ["werewolf"] = new WerewolfGame(),
                    ["chess"] = new ChessGame()
                };

        await client.StartAsync(cancelSource.Token);
            }).GetAwaiter().GetResult();
        }
    }
}
