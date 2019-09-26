using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // Marks a command to require the user executing it to have an existing account.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireUserAccountAttribute : PreconditionAttribute
    {
        private readonly bool _autoBuild;
        public RequireUserAccountAttribute(bool autoBuild = true)
        {
            _autoBuild = autoBuild;
        }
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            OriCommandContext Context = context as OriCommandContext;
            if (Context.Account == null)
            {
                if (_autoBuild)
                {
                    Console.WriteLine("[Debug] -- User automatically built; Appended to OriCommandContext. --");
                    Context.Account = Context.Container.GetOrAddUser(Context.User);
                    return PreconditionResult.FromSuccess();
                }

                return PreconditionResult.FromError("This command cannot be executed without a user account.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
