using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    // Once ready, you can then reference the command context to load the item information
    public sealed class ItemTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (ItemHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ItemHelper.GetItem(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find an Item with the specified ID."));
        }
    }

    public sealed class ArcadeUserTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeContext context))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "Invalid command context specified"));

            if (ulong.TryParse(input, out ulong uId))
                if (context.Data.Users.Values.ContainsKey(uId))
                    return Task.FromResult(TypeReaderResult.FromSuccess(context.Data.Users.Values[uId]));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find the specified account."));
        }
    }
}
