using Discord;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // internal means that it cannot be accessed once it's compiled

    // TODO: Separate all of the files in 'Orikivo' namespace into their proper locations.
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                ConsoleConfig consoleConfig = ConsoleConfig.Default;
                consoleConfig.Title = $"Orikivo: {OriGlobal.ClientVersion}";
                consoleConfig.BackgroundColor = ConsoleColor.Black;
                consoleConfig.TextColor = ConsoleColor.White;
                //consoleConfig.OutputPath = "../logs/";

                // A simple little boot tune for fun:
                /*
                Console.Beep(800, 300);
                Thread.Sleep(200);
                Console.Beep(700, 200);
                Console.Beep(600, 200);
                Console.Beep(900, 200);
                Console.Beep(700, 200);
                Console.Beep(800, 200);
                */

                OriClientBuilder builder = new OriClientBuilder();
                builder.ConsoleConfig = consoleConfig;
                builder.LogConfig = LogConfig.Default;

                builder.AddTypeReader<GameMode>(new GameModeTypeReader());
                builder.AddTypeReader<ReportTag>(new ReportTagTypeReader());
                builder.AddTypeReader<GuildEvent>(new GuildEventTypeReader());
                builder.AddTypeReader<RasterizerType>(new RasterizerTypeTypeReader());
                builder.AddTypeReader<MeritGroup>(new MeritGroupTypeReader());

                builder.AddModule<MiscModule>();
                builder.AddModule<MessyModule>();
                builder.AddModule<WorldModule>();

                OriClient client = builder.Build();
                await client.SetGameAsync("Minecraft", activity: ActivityType.Listening);
                await client.SetStatusAsync(UserStatus.DoNotDisturb);
                await client.StartAsync();
            }).GetAwaiter().GetResult();
        }
    }
}
