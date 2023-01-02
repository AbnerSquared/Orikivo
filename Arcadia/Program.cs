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
using Arcadia.Interactions;
using Discord.Interactions;
using Arcadia.Converters;

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

                var builder = new InteractionClientBuilder();

                builder.Services
                    .AddSingleton<GameManager>()
                    .AddSingleton<LocaleProvider>()
                    .AddSingleton<InfoFormatter>()
                    .AddSingleton<InfoService>()
                    .AddSingleton<ArcadeContainer>()
                    .AddSingleton<LogService>()
                    .AddSingleton<InteractionHandler>()
                    .AddSingleton<EventHandler>()
                    .AddSingleton<CasinoService>();

                builder.SocketConfig = Orikivo.DiscordConfig.DefaultSocketConfig;
                builder.InteractionConfig = Orikivo.DiscordConfig.DefaultInteractionConfig;

                builder
                    .AddTypeConverter<Item>(new ItemTypeConverter())
                    .AddTypeConverter<Badge>(new BadgeTypeConverter())
                    .AddTypeConverter<Recipe>(new RecipeTypeConverter())
                    .AddTypeConverter<Quest>(new QuestTypeConverter())
                    .AddTypeConverter<Shop>(new ShopTypeConverter())
                    .AddTypeConverter<ArcadeUser>(new ArcadeUserTypeConverter())
                    .AddTypeConverter<Wager>(new WagerTypeConverter())
                    .AddEnumTypeConverter<Casing>()
                    .AddEnumTypeConverter<FontType>()
                    .AddEnumTypeConverter<PaletteType>()
                    .AddEnumTypeConverter<BorderAllow>()
                    .AddEnumTypeConverter<ImageScale>()
                    .AddEnumTypeConverter<CardGroup>()
                    .AddEnumTypeConverter<Gamma>()
                    .AddEnumTypeConverter<BorderEdge>()
                    .AddEnumTypeConverter<DoublerWinMethod>()
                    .AddEnumTypeConverter<StackTraceMode>()
                    .AddEnumTypeConverter<ChessOwner>()
                    .AddEnumTypeConverter<Privacy>()
                    .AddEnumTypeConverter<RouletteBetMode>();

                builder
                    .AddTypeReader<Item>(new ItemTypeReader())
                    .AddTypeReader<Badge>(new BadgeTypeReader())
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
                    .AddEnumTypeReader<DoublerWinMethod>()
                    .AddEnumTypeReader<StackTraceMode>()
                    .AddEnumTypeReader<ChessOwner>()
                    .AddEnumTypeReader<Privacy>()
                    .AddEnumTypeReader<RouletteBetMode>();

                builder
                    //.AddModule<Core>()
                    //.AddModule<Modules.Casino>()
                    //.AddModule<Modules.Multiplayer>()
                    .AddModule<Common>();
                    //.AddModule<Economy>()
                    //.AddModule<Records>();

                InteractionClient client = builder.Build();

                client.Status = UserStatus.Online;
                client.Activity = new ActivityConfig
                {
                    Name = "your requests",
                    Type = ActivityType.Listening
                };

                client.Provider.GetRequiredService<EventHandler>();
                client.Provider.GetRequiredService<InteractionHandler>();

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
