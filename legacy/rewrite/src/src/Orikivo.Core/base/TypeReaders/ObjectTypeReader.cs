using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class ObjectTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            object result = input;
            if (input.Exists())
            {
                if (input.TryParseEmpty(out object empty))
                    result = empty;
                else if (int.TryParse(input, out int i))
                    result = i;
                else if (bool.TryParse(input, out bool b))
                    result = b;
                else if (ulong.TryParse(input, out ulong u))
                    result = u;
                else if (DateTime.TryParse(input, out DateTime date))
                    result = date;
                else if (TimeSpan.TryParse(input, out TimeSpan span))
                    result = span;
                else if (Emote.TryParse(input, out Emote emote))
                    result = emote;
                else if (input.TryParseList(out List<object> list))
                    result = list;
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(result));
        }
    }
}
