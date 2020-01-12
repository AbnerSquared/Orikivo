using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    public class RequireHuskAttribute : PreconditionAttribute
    {
        public HuskFlag Flag { get; }

        public RequireHuskAttribute(HuskFlag flag)
        {
            Flag = flag;
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
