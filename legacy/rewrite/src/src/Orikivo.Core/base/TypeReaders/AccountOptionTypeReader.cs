using Discord;
using Discord.Commands;
using Orikivo.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class AccountOptionTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            AccountOption result;
            if (int.TryParse(input, out int id))
            {
                if (OptionsHelper.TryParseOption(id, out result))
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
                }
            }
            if (input.TryParseEmoji(out Emoji emoji))
            {
                if (OptionsHelper.TryParseOption(emoji, out result))
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
                }
            }
            if (OptionsHelper.TryParseOption(input, out result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an AccountOption."));
        }
    }
}
