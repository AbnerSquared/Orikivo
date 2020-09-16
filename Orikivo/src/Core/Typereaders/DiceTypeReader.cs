using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // d = roll 6
    // d{n} = roll n
    // {c}d = roll 6 c times
    // {c}d{n} = roll n c times
    // TODO: Create the DiceTypeReader, which should be able to handle {amount}d(OR)D{sides}[] (EX: 1d2 1D2 100d
    public class DiceTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
