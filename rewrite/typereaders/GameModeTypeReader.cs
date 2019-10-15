using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GameModeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            GameMode result;
            if (int.TryParse(input, out int i))
                if (Enum.TryParse(input, out result))
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
            if (EnumParser.TryParse(input, out result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input did not match any parsable value."));
        }
    }
}
