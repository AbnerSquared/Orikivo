using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // make a DiceTypeReader
    // which can handle d10 100d100 20d20
    public class EntityDisplayFormatTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            EntityDisplayFormat result;
            if (int.TryParse(input, out int i))
                if (Enum.TryParse(input, out result))
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
            if (EnumParser.TryParse(input, out result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input did not match any parsable value."));
        }
    }
}
