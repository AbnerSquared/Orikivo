using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    public sealed class ShopTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeContext context))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "Invalid command context specified"));

            if (context.Account == null)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "The executing user does not have an account."));

            if (ShopHelper.ExistsFor(context.Account, input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ShopHelper.GetShop(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "I couldn't find a **Shop** under that ID."));
        }
    }
}
