using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // Marks a command to require a specified trust level in order to execute.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AccessAttribute : PreconditionAttribute
    {
        public TrustLevel Level { get; }
        public bool DevOverride { get; }
        // in short, this makes it so that commands can prevent the developer from overriding guild owner only options.
        public AccessAttribute(TrustLevel level, bool devOverride = true)
        {
            Level = level;
            DevOverride = devOverride;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            OriCommandContext Context = context as OriCommandContext;
            switch(Level)
            {
                case TrustLevel.Dev:
                    if (Context.User.Id != OriGlobal.DevId)
                        return PreconditionResult.FromError("This command can only be executed by a developer.");
                    break;
                case TrustLevel.Owner:
                    if (Context.User.Id != OriGlobal.DevId && Context.User.Id != Context.Server.OwnerId)
                        return PreconditionResult.FromError("This command can only be executed by this guild's owner.");
                    break;
                case TrustLevel.Inherit:
                    if (Context.Server.Options.TrustRoleId.HasValue ?
                        Context.User.Id != OriGlobal.DevId && Context.User.Id != Context.Server.OwnerId && !Context.Guild.Users.Any(x => x.Roles.Contains(Context.Guild.GetRole(Context.Server.Options.TrustRoleId.Value)) && x.Id == Context.User.Id) :
                        Context.User.Id != OriGlobal.DevId && Context.User.Id != Context.Server.OwnerId)
                        return PreconditionResult.FromError("This command can only be executed by trusted users within this guild.");
                    break;
                default:
                    throw new Exception("The specified TrustLevel doesn't exist.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
