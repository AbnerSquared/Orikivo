using Discord.Interactions;
using System;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Converters
{
    public sealed class ArcadeUserTypeConverter : TypeConverter<ArcadeUser>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.User;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (!(option.Value is IUser user))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Could not convert the specified input to type {nameof(ArcadeUser)}"));

            if (context.Data.Users.Values.ContainsKey(user.Id))
                return Task.FromResult(TypeConverterResult.FromSuccess(context.Data.Users.Values[user.Id]));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "The user specified does not have an account."));
        }
    }
}
