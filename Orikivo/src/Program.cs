using Discord;
using Orikivo.Drawing;
using Orikivo.Framework;
using System;
using System.Threading;
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
                var _cancelSource = new CancellationTokenSource();
                var layout = new ConsoleLayout
                {
                    Title = $"Orikivo: {OriGlobal.ClientVersion}",
                    BackgroundColor = ConsoleColor.DarkCyan,
                    ForegroundColor = ConsoleColor.Cyan,
                    CursorVisible = false
                };

                layout.Set();

                var builder = new ClientBuilder();
                builder.SetDefaultServices();

                builder
                    .AddTypeReader<ReportTag>(new EnumTypeReader<ReportTag>())
                    .AddTypeReader<EventType>(new EnumTypeReader<EventType>())
                    .AddTypeReader<RasterizerType>(new EnumTypeReader<RasterizerType>())
                    .AddTypeReader<MeritGroup>(new EnumTypeReader<MeritGroup>())
                    .AddTypeReader<Operator>(new EnumTypeReader<Operator>());

                builder
                    .AddModule<CoreModule>()
                    .AddModule<MessyModule>()
                    .AddModule<DigitalModule>()
                    .AddModule<GraphicsModule>()
                    .AddModule<Actions>();

                Client client = builder.Build()
                    .WithStatus(UserStatus.DoNotDisturb)
                    .WithActivity("Minecraft", type: ActivityType.Listening);

                client.EnsureDefaultServices();
                await client.StartAsync(_cancelSource.Token);
            }).GetAwaiter().GetResult();
        }
    }
}
