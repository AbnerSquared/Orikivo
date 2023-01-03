using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia.Commands
{
    public sealed class BadgeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeContext context))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "Invalid command context specified"));

            if (input.Equals("recent", StringComparison.OrdinalIgnoreCase))
            {
                Badge newest = MeritHelper.GetNewestUnlocked(context.Account);

                if (newest == null)
                {
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "You haven't unlocked any merits recently."));
                }

                return Task.FromResult(TypeReaderResult.FromSuccess(newest));
            }

            if (MeritHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(MeritHelper.GetMerit(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Merit with the specified ID."));
        }
    }
}
