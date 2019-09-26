using Discord;
using Discord.Commands;
using Orikivo.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class EmojiTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            if (input.TryParseEmoji(out Emoji result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an Emoji."));
        }
    }
}