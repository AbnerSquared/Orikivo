using Discord.Commands;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{
    /// <summary>
    /// A <see cref="PreconditionAttribute"/> that requires the executing user to have an account.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireUserAttribute : PreconditionAttribute
    {
        private readonly bool _autoBuild;
        public AccountHandling Handling { get; }

        public RequireUserAttribute(AccountHandling handling = AccountHandling.ReadWrite, bool autoBuild = true)
        {
            Handling = handling;
            _autoBuild = autoBuild;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            DesyncContext Context = context as DesyncContext;
            if (Context.Account == null)
            {
                if (_autoBuild)
                {
                    Context.Container.GetOrAddUser(Context.User);
                    return PreconditionResult.FromSuccess();
                }

                return PreconditionResult.FromError("This command cannot be executed without a user account.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
