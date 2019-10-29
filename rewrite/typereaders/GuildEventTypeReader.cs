using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GuildEventTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            if (Enum.TryParse(input, true, out GuildEvent result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Parse failed"));
        }
    }
}
