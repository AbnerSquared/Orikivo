using Discord.Commands;
using Orikivo.Unstable;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class RequireHuskAttribute : PreconditionAttribute
    {
        public HuskState State { get; }

        public RequireHuskAttribute(HuskState state)
        {
            State = state;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            OriCommandContext Context = context as OriCommandContext;
            Context.Container.GetOrAddUser(Context.User);

            // Context.Account.Husk;

            return PreconditionResult.FromSuccess();
        }
    }
}
