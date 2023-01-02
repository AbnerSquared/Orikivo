using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public sealed class InteractionEnumTypeReader<TEnum> : Discord.Interactions.TypeReader
        where TEnum : struct
    {
        public override bool CanConvertTo(Type type)
        {
            return type == typeof(TEnum);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (!typeof(TEnum).IsEnum)
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful,
                    "The specified TEnum type is not an explicit Enum."));

            if (Enum.TryParse(input, true, out TEnum result))
                return Task.FromResult(TypeConverterResult.FromSuccess(result));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed,
                $"The input specified could not be parsed into the Enum '{typeof(TEnum).Name}'."));
        }
    }
}
