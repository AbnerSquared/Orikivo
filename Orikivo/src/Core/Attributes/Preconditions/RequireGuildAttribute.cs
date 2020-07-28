using Discord.Commands;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{

    /// <summary>
    /// A precondition that marks a command to require the guild it was executed in to have an account.
    /// </summary>
    public class RequireGuildAttribute : PreconditionAttribute
    {
        public AccountHandling Handling { get; }

        public RequireGuildAttribute(AccountHandling handling = AccountHandling.ReadWrite)
        {
            Handling = handling;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            DesyncContext Context = context as DesyncContext;
            Context.Server ??= Context.Container.GetOrAddGuild(Context.Guild);

            return PreconditionResult.FromSuccess();
        }
    }
}
