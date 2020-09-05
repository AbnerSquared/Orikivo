using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    public sealed class ShopTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (ShopHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ShopHelper.GetShop(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Shop with the specified ID."));
        }
    }
}
