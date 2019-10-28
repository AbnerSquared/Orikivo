using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class UserExtensions
    {
        /// <summary>
        /// Adds a role if the user does not already have it. Otherwise, the role is removed.
        /// </summary>
        public static async Task AddOrRemoveRoleAsync(this IGuildUser user, IRole role, RequestOptions options = null)
        {
            if (user.RoleIds.Contains(role.Id))
                await user.RemoveRoleAsync(role, options);
            else
                await user.AddRoleAsync(role, options);
        }
    }
}
