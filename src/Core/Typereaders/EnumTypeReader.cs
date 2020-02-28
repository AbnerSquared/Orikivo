using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Defines a reader class for <see cref="Enum"/> values.
    /// </summary>
    /// <typeparam name="TEnum">Represents the <see cref="Enum"/> to compare.</typeparam>
    public class EnumTypeReader<TEnum> : TypeReader where TEnum : struct
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (!typeof(TEnum).IsEnum)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful,
                    "The specified TEnum type is not an explicit Enum."));

            if (Enum.TryParse(input, true, out TEnum result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                $"The input specified could not be parsed into the Enum '{typeof(TEnum).Name}'."));
        }
    }
}
