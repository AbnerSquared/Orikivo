using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class ReportFlagTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            ReportFlag result;
            if (int.TryParse(input, out int i))
                if (Enum.TryParse(input, out result))
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
            if (EnumParser.TryParseEnum(input, out result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input did not match any parsable value."));
        }
    }
}
