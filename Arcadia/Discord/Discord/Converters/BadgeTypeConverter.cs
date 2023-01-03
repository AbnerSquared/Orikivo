using Discord.Interactions;
using System;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Converters
{
    public sealed class BadgeTypeConverter : TypeConverter<Badge>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (!(option.Value is string input))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Could not convert the specified input to string"));

            if (input.Equals("recent", StringComparison.OrdinalIgnoreCase))
            {
                Badge newest = MeritHelper.GetNewestUnlocked(context.Account);

                if (newest == null)
                {
                    return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "You haven't earned any badges recently."));
                }

                return Task.FromResult(TypeConverterResult.FromSuccess(newest));
            }

            if (MeritHelper.Exists(input))
                return Task.FromResult(TypeConverterResult.FromSuccess(MeritHelper.GetMerit(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Could not find a badge with the specified ID."));
        }
    }
}
