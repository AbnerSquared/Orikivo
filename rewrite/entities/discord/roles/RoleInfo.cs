using Discord;
using Discord.WebSocket;
using System.Linq;

namespace Orikivo
{
    public class RoleInfo : RoleBaseInfo
    {
        public RoleInfo(SocketRole role) : base(role)
        {
            Guild = new GuildBaseInfo(role.Guild);
            UserCount = role.Members.Count();
            GuildUserCount = role.Guild.MemberCount;
            Position = role.Position;
            Flags = new RoleFlags(role.IsHoisted, role.IsManaged, role.IsMentionable);
            Permissions = role.Permissions;
        }
        public GuildBaseInfo Guild { get; }

        public int Position { get; }

        public int UserCount { get; }
        public int GuildUserCount { get; }

        public RoleFlags Flags { get; }
        
        public GuildPermissions Permissions { get; }
    }
}