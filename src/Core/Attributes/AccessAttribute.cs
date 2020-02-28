using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{
    /// <summary>
    /// Represents a <see cref="PreconditionAttribute"/> that enforces an <see cref="AccessLevel"/> requirement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AccessAttribute : PreconditionAttribute
    {
        /// <summary>
        /// Represents the required authority in order to access the method that the <see cref="AccessAttribute"/> is attached to.
        /// </summary>
        public AccessLevel Level { get; }

        /// <summary>
        /// Determines if a developer is allowed to override the method that the <see cref="AccessAttribute"/> is attached to.
        /// </summary>
        public bool DevOverride { get; }

        /// <summary>
        /// Initializes a new <see cref="AccessAttribute"/> with the specified <see cref="AccessLevel"/>.
        /// </summary>
        public AccessAttribute(AccessLevel level, bool devOverride = true)
        {
            Level = level;
            DevOverride = devOverride;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            OriCommandContext Context = context as OriCommandContext;
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
                    throw new ArgumentException("The specified AccessLevel does not exist.");
            }

            return PreconditionResult.FromSuccess();
        }

        private bool CheckDev(ulong userId)
            => OriGlobal.DevId == userId;

        private bool CheckOwner(ulong userId, OriCommandContext ctx)
            => DevOverride ? CheckDev(userId) : ctx.Server.OwnerId == userId;

        // NOTE: It could be possible to not have to get the guild's role for it, but yet again, it ensures the trust role exists.
        // Maybe the trust role can be ensured elsewhere, since Discord should auto-remove deleted roles.
        // OLD => x.Roles.Contains(ctx.Guild.GetRole(ctx.Server.Options.TrustRoleId.Value))
        private bool CheckInherit(ulong userId, OriCommandContext ctx)
            => DevOverride ? CheckDev(userId) : ctx.Server.Options.TrustRoleId.HasValue ?
               ctx.Guild.Users.Any(x => x.Id == userId && x.Roles.Any(y => y.Id == ctx.Server.Options.TrustRoleId.Value)) :
               ctx.Server.OwnerId == userId;
    }
}
