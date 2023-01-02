using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Arcadia.Interactions
{
    public sealed class ShopTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(Shop).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (context.Account == null)
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "The executing user does not have an account."));

            if (ShopHelper.ExistsFor(context.Account, input))
                return Task.FromResult(TypeConverterResult.FromSuccess(ShopHelper.GetShop(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "I could not find a **Shop** using that ID."));
        }
    }
}
