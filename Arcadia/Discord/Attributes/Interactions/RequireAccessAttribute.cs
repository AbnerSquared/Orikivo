﻿using Discord.Interactions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Arcadia;
using Orikivo;
using Discord;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia.Interactions
{
    /// <summary>
    /// Represents a <see cref="PreconditionAttribute"/> that enforces an <see cref="AccessLevel"/> requirement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireAccessAttribute : PreconditionAttribute
    {
        /// <summary>
        /// Represents the required authority in order to access the method that the <see cref="RequireAccessAttribute"/> is attached to.
        /// </summary>
        public AccessLevel Level { get; }

        /// <summary>
        /// Determines if a developer is allowed to override the method that the <see cref="RequireAccessAttribute"/> is attached to.
        /// </summary>
        public bool DevOverride { get; }

        /// <summary>
        /// Initializes a new <see cref="RequireAccessAttribute"/> with the specified <see cref="AccessLevel"/>.
        /// </summary>
        public RequireAccessAttribute(AccessLevel level, bool devOverride = true)
        {
            Level = level;
            DevOverride = devOverride;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider provider)
        {
            if (!(context is ArcadeInteractionContext Context))
                throw new Exception("Unknown CommandContext type");

            switch(Level)
            {
                case AccessLevel.Dev:
                    if (!CheckDev(Context.User.Id))
                        return PreconditionResult.FromError("This command can only be executed by a developer.");
                    break;

                case AccessLevel.Owner:
                    if (!CheckOwner(Context.User.Id, Context))
                        return PreconditionResult.FromError("This command can only be executed by this guild's owner.");
                    break;

                case AccessLevel.Inherit:
                    if (!CheckInherit(Context.User.Id, Context))
                        return PreconditionResult.FromError("This command can only be executed by trusted users within this guild.");
                    break;

                default:
                    return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromSuccess();
        }

        private static bool CheckDev(ulong userId)
            => Orikivo.Constants.DevId == userId;

        private bool CheckOwner(ulong userId, ArcadeInteractionContext ctx)
            => DevOverride ? CheckDev(userId) : ctx.Guild?.OwnerId == userId;

        private bool CheckInherit(ulong userId, ArcadeInteractionContext ctx)
        {
            if (DevOverride)
                return CheckDev(userId);

            if (ctx.Server == null)
                return false;

            if (ctx.Server.Config.TrustedRoleId.HasValue)
            {
                return ctx.Guild.Users.Any(x =>
                    x.Id == userId && x.Roles.Any(y => y.Id == ctx.Server.Config.TrustedRoleId.Value));
            }

            return ctx.Server.OwnerId == userId;
        }
    }
}
