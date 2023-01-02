using Discord.Interactions;
using System;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Converters
{
    public sealed class ShopTypeConverter : TypeConverter<Shop>
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

            if (ShopHelper.ExistsFor(context.Account, input))
                return Task.FromResult(TypeConverterResult.FromSuccess(ShopHelper.GetShop(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "I could not find a **Shop** using that ID."));
        }
    }
}
