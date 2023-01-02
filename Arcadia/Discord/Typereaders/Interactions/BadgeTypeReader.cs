using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Arcadia.Interactions
{
    public sealed class BadgeTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(Badge).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

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
