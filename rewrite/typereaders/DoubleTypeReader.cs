using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class DoubleTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {

            try
            {
                (double d, IntLengthType? type) info = OriRegex.TryParseCustomDouble(input);
                if (info.type.HasValue)
                {
                    switch (info.type)
                    {
                        case IntLengthType.Minutes:
                            info.d = info.d * 60;
                            break;
                        case IntLengthType.Hours:
                            info.d = info.d * 60 * 60;
                            break;
                        case IntLengthType.Days:
                            info.d = info.d * 60 * 60 * 24;
                            break;
                    }
                }

                return Task.FromResult(TypeReaderResult.FromSuccess(info.d));
            }
            catch(Exception)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Double could not be parsed."));
            }
        }
    }
}
