using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Arcadia.Interactions
{
    public sealed class WagerTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(Wager).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeInteractionContext context))
                throw new Exception("Invalid command context was initialized");

            input = input.ToLower();
            long chipBalance = context.Account.ChipBalance;

            return Task.FromResult(input switch
            {
                "all" => TypeConverterResult.FromSuccess(new Wager(chipBalance)),
                "half" => TypeConverterResult.FromSuccess(new Wager(chipBalance / 2)),
                "quarter" => TypeConverterResult.FromSuccess(new Wager(chipBalance / 4)),
                _ when input.EndsWith('%')
                       && long.TryParse(input[..^1], out long percent)
                       && percent > 0
                       && percent <= 100 => TypeConverterResult.FromSuccess(new Wager(FromPercent(chipBalance, percent))),
                _ when long.TryParse(input, out long wager) => TypeConverterResult.FromSuccess(new Wager(wager)),
                _ => TypeConverterResult.FromError(InteractionCommandError.ParseFailed, "An invalid wager input was given.")
            });
        }

        private static long FromPercent(long wager, long percent)
        {
            return (long)Math.Floor((percent / (double)100) * wager);
        }
    }
}
