using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia.Commands
{
    // Once ready, you can then reference the command context to load the item information
    public sealed class ItemTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (ItemHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ItemHelper.GetItem(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "I couldn't find an **Item** under that ID."));
        }
    }
}
