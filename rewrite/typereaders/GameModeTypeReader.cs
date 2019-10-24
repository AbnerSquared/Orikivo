using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GameModeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            if (Enum.TryParse(input, true, out GameMode result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input did not match any parsable value."));
        }
    }
}
