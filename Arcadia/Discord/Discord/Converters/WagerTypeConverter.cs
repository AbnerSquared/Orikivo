using Discord.Interactions;
using System;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Converters
{
    public sealed class WagerTypeConverter : TypeConverter<Wager>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (context.Account == null)
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "The executing user does not have an account."));

            if (!(option.Value is string input))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Could not convert the specified input to string"));



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
