using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Scrap alongside EntityDisplayFormat.
    public class EntityDisplayFormatTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            if (Enum.TryParse(input, true, out EntityDisplayFormat result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input did not match any parsable value."));
        }
    }
}
