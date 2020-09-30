using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Arcadia
{
    public sealed class ArcadeUserTypeReader : UserTypeReader<SocketUser>
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeContext context))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "Invalid command context specified"));

            if (ulong.TryParse(input, out ulong uId))
                if (context.Data.Users.Values.ContainsKey(uId))
                    return Task.FromResult(TypeReaderResult.FromSuccess(context.Data.Users.Values[uId]));

            ArcadeUser match = context.Data.Users.Values.Values.FirstOrDefault(x => x.Username.Equals(input, StringComparison.OrdinalIgnoreCase));

            if (match != null)
                return Task.FromResult(TypeReaderResult.FromSuccess(match));

            TypeReaderResult result = base.ReadAsync(ctx, input, provider).ConfigureAwait(false).GetAwaiter().GetResult();

            if (result.IsSuccess && result.BestMatch is SocketUser best && context.TryGetUser(best.Id, out match))
                return Task.FromResult(TypeReaderResult.FromSuccess(match));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find the specified account."));
        }
    }
}
