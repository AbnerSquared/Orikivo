using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequirePermissionsAttribute : PreconditionAttribute
    {
        public List<GuildPermission> Permissions { get; }
        public RequirePermissionsAttribute(params GuildPermission[] permissions)
        {
            Permissions = new List<GuildPermission>();
            if (permissions.Length == 0)
                throw new Exception("RequirePermissionsAttribute requires that at least one permission is set.");
            foreach (GuildPermission permission in permissions)
            {
                if (Permissions.Contains(permission))
                    continue;
                Permissions.Add(permission);
            }
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            IGuildUser clientUser = context.Guild.GetUserAsync(context.Client.CurrentUser.Id).Result;
            if (clientUser == null)
                throw new Exception("The IGuildUser variant of the current client could be found.");
            if (clientUser.GuildPermissions.Has(GuildPermission.Administrator)) // admin bypasses everything
                return PreconditionResult.FromSuccess();
            List<GuildPermission> missingPermissions = new List<GuildPermission>();
            foreach (GuildPermission permission in Permissions)
            {
                if (!clientUser.GuildPermissions.Has(permission))
                    missingPermissions.Add(permission);
            }
            if (missingPermissions.Count > 0)
                return PreconditionResult.FromError($"Orikivo requires the following permissions: {string.Join(", ", missingPermissions.Select(x => x.ToString()))}");
            return PreconditionResult.FromSuccess();
        }
    }
}
