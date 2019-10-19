﻿using Discord;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // internal means that it cannot be accessed once it's compiled

    // TODO: Separate all of the files in 'Orikivo' namespace into their proper locations.
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                OriConsoleConfig oriConsoleConfig = OriConsoleConfig.Default;
                oriConsoleConfig.Title = $"Orikivo: {Assembly.GetEntryAssembly().GetName().Version.ToString()}";
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
                OriClientBuilder builder = new OriClientBuilder();
                builder.ConsoleConfig = oriConsoleConfig;
                builder.LogConfig = OriLogConfig.Default;
                builder.AddTypeReader<EntityDisplayFormat>(new EntityDisplayFormatTypeReader());
                builder.AddTypeReader<GameMode>(new GameModeTypeReader());
                builder.AddTypeReader<ReportTag>(new ReportTagTypeReader());
                //oriClientBuilder.AddTypeReader<double>(new DoubleTypeReader());

                builder.AddModule<MiscModule>();
                builder.AddModule<MessyModule>();

                OriClient oriClient = builder.Build();
                await oriClient.SetGameAsync("Minecraft", activity: ActivityType.Listening);
                await oriClient.SetStatusAsync(UserStatus.DoNotDisturb);
                await oriClient.StartAsync();
                

                
               
            }).GetAwaiter().GetResult();
        }
    }
}
