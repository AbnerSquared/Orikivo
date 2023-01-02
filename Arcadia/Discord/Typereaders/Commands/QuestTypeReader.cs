using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia.Commands
{
    public sealed class QuestTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (QuestHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(QuestHelper.GetQuest(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Quest with the specified ID."));
        }
    }
}
