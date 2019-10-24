using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Create the DiceTypeReader, which should be able to handle {amount}d(OR)D{sides}[]
    public class DiceTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
            => throw new NotImplementedException();
    }
}
