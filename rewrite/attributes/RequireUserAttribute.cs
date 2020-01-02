﻿using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// A precondition that marks a command to require the user executing it to have an account.
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
            OriCommandContext Context = context as OriCommandContext;
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
