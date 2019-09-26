using Discord;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // internal means that it cannot be accessed once it's compiled
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                OriConsoleConfig oriConsoleConfig = OriConsoleConfig.Default;
                oriConsoleConfig.Title = $"Orikivo: {OriGlobal.ClientVersion}";
                oriConsoleConfig.BackgroundColor = ConsoleColor.Black;
                oriConsoleConfig.TextColor = ConsoleColor.White;

                // boot tune
                //Console.Beep(800, 300);
                //Thread.Sleep(200);
                //Console.Beep(700, 200);
                //Console.Beep(600, 200);
                //Console.Beep(900, 200);
                //Console.Beep(700, 200);
                //Console.Beep(800, 200);

                // find a place for args
                OriClientBuilder oriClientBuilder = new OriClientBuilder();
                oriClientBuilder.ConsoleConfig = oriConsoleConfig;
                oriClientBuilder.LoggerConfig = OriLoggerConfig.Default;
                oriClientBuilder.AddTypeReader<EntityDisplayFormat>(new EntityDisplayFormatTypeReader());
                oriClientBuilder.AddTypeReader<GameMode>(new GameModeTypeReader());
                oriClientBuilder.AddTypeReader<ReportFlag>(new ReportFlagTypeReader());
                //oriClientBuilder.AddTypeReader<double>(new DoubleTypeReader());

                oriClientBuilder.AddModule<AlphaModule>();

                OriClient oriClient = oriClientBuilder.Build();
                await oriClient.SetGameAsync("Minecraft", activity: ActivityType.Listening);
                await oriClient.SetStatusAsync(UserStatus.DoNotDisturb);
                await oriClient.StartAsync();
                

                
               
            }).GetAwaiter().GetResult();
        }
    }
}
