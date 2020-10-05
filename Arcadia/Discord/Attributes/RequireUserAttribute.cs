using Discord.Commands;
using Orikivo;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia
{
    // TODO: Implement easy class overrides of results to allow for different context
    /// <summary>
    /// A <see cref="PreconditionAttribute"/> that requires the executing user to have an account.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
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
            var Context = context as ArcadeContext;
            if (Context.Account == null)
            {
                if (_autoBuild)
                {
                    Context.GetOrAddUser(Context.User);
                    return PreconditionResult.FromSuccess();
                }

                return PreconditionResult.FromError("This command cannot be executed without a user account.");
            }

            return PreconditionResult.FromSuccess();
        }
    }

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
            if (!(context is ArcadeContext Context))
                throw new Exception("Expected ArcadeContext");

            Context.Server ??= Context.GetOrAddGuild(context.Guild);

            return PreconditionResult.FromSuccess();
        }
    }
}
