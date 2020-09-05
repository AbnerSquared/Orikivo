using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    public sealed class MeritTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (MeritHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(MeritHelper.GetMerit(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Merit with the specified ID."));
        }
    }
}
