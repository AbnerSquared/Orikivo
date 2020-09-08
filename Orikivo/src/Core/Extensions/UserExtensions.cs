using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    public static class UserExtensions
    {
        /// <summary>
        /// Adds or removes the specified <paramref name="role" /> from this user.
        /// </summary>
        public static async Task ToggleRoleAsync(this IGuildUser user, IRole role, RequestOptions options = null)
        {
            if (user.RoleIds.Contains(role.Id))
                await user.RemoveRoleAsync(role, options);
            else
                await user.AddRoleAsync(role, options);
        }
    }
}
