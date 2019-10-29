using Discord;

namespace Orikivo
{
    public static class DiscordUtils
    {
        public static RoleProperties MuteRoleProperties
        {
            get
            {
                RoleProperties role = new RoleProperties
                {
                    Name = "Muted",
                    Permissions = new GuildPermissions(66560),
                    Mentionable = false,
                    Hoist = false
                };
                return role;
            }
        }
    }
}
