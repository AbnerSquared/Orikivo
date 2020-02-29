using Discord;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Defines the entry point for <see cref="Orikivo"/>.
    /// </summary>
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

                ClientBuilder builder = new ClientBuilder();
                builder.ConsoleConfig = consoleConfig;
                builder.LogConfig = LogConfig.Default;
                ClientBuilder.SetDefaultServices(builder.Services);

                builder.AddTypeReader<GameMode>(new EnumTypeReader<GameMode>());
                builder.AddTypeReader<ReportTag>(new EnumTypeReader<ReportTag>());
                builder.AddTypeReader<EventType>(new EnumTypeReader<EventType>());
                builder.AddTypeReader<RasterizerType>(new EnumTypeReader<RasterizerType>());
                builder.AddTypeReader<MeritGroup>(new EnumTypeReader<MeritGroup>());

                builder.AddModule<MiscModule>();
                builder.AddModule<MessyModule>();
                builder.AddModule<DigitalModule>();
                builder.AddModule<HuskActions>();

                Client client = builder.Build();
                //Client.EnsureDefaultServices(client.Provider);
                await client.SetGameAsync("Minecraft", activity: ActivityType.Listening);
                await client.SetStatusAsync(UserStatus.DoNotDisturb);
                await client.StartAsync();
            }).GetAwaiter().GetResult();
        }
    }
}
