using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    public sealed class RecipeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (CraftHelper.RecipeExists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(CraftHelper.GetRecipe(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "I could not find a **Recipe** using that ID."));
        }
    }
}
