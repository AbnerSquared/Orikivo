using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Arcadia.Interactions
{
    public sealed class ItemTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(Item).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (ItemHelper.Exists(input))
                return Task.FromResult(TypeConverterResult.FromSuccess(ItemHelper.GetItem(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "I couldn't find an **Item** under that ID."));
        }
    }
}
