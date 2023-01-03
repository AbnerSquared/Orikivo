using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Arcadia.Interactions
{
    public sealed class ArcadeUserTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(ArcadeUser).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Invalid command context specified"));

            if (ulong.TryParse(input, out ulong uId))
                if (context.Data.Users.Values.ContainsKey(uId))
                    return Task.FromResult(TypeConverterResult.FromSuccess(context.Data.Users.Values[uId]));

            ArcadeUser match = context.Data.Users.Values.Values.FirstOrDefault(x => x.Username.Equals(input, StringComparison.OrdinalIgnoreCase));

            if (match != null)
                return Task.FromResult(TypeConverterResult.FromSuccess(match));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "The user specified does not have an account."));
        }
    }
}
