using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    public sealed class WagerTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeContext context))
                throw new Exception("Invalid command context was initialized");

            input = input.ToLower();
            long chipBalance = context.Account.ChipBalance;

            return Task.FromResult(input switch
            {
                "all" => TypeReaderResult.FromSuccess(new Wager(chipBalance)),
                "half" => TypeReaderResult.FromSuccess(new Wager(chipBalance / 2)),
                "quarter" => TypeReaderResult.FromSuccess(new Wager(chipBalance / 4)),
                _ when input.EndsWith('%')
                       && long.TryParse(input[..^1], out long percent)
                       && percent > 0
                       && percent <= 100 => TypeReaderResult.FromSuccess(new Wager(FromPercent(chipBalance, percent))),
                _ when long.TryParse(input, out long wager) => TypeReaderResult.FromSuccess(new Wager(wager)),
                _ => TypeReaderResult.FromError(CommandError.ParseFailed, "An invalid wager input was given.")
            });
        }

        private static long FromPercent(long wager, long percent)
        {
            return (long)Math.Floor((percent / (double)100) * wager);
        }
    }
}