using Discord.Interactions;
using System;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Converters
{
    public sealed class RecipeTypeConverter : TypeConverter<Recipe>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (!(option.Value is string input))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Could not convert the specified input to string"));

            if (CraftHelper.RecipeExists(input))
                return Task.FromResult(TypeConverterResult.FromSuccess(CraftHelper.GetRecipe(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "I could not find a **Recipe** using that ID."));

        }
    }
}
