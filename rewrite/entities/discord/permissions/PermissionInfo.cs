using Discord;

namespace Orikivo
{
    public class PermissionInfo
    {
        public GuildPermission Permission { get; }
        public string Summary { get; }
        public PermissionGuildInfo GuildInfo { get; }
    }
}