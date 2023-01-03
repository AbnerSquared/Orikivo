using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Arcadia.Interactions
{
    public sealed class RecipeTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(Recipe).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (CraftHelper.RecipeExists(input))
                return Task.FromResult(TypeConverterResult.FromSuccess(CraftHelper.GetRecipe(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "I could not find a **Recipe** using that ID."));
        }
    }
}
