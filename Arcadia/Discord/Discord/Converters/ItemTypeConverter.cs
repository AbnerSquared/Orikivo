using Discord.Interactions;
using System;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Converters
{
    public sealed class ItemTypeConverter : TypeConverter<Item>
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (!(option.Value is string input))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Could not convert the specified input to string"));

            if (ItemHelper.Exists(input))
                return Task.FromResult(TypeConverterResult.FromSuccess(ItemHelper.GetItem(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "I couldn't find an **Item** under that ID."));
        }
    }
}
