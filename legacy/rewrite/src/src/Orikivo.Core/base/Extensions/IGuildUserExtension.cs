using Discord;
using Discord.WebSocket;
using Orikivo.Storage;
using System;
using System.Linq;

namespace Orikivo
{
    public static class SocketGuildUserExtension
    {
        public static bool HasRole(this SocketGuildUser user, SocketRole r) =>
            user.Roles.Contains(r);

        public static bool HasRole(this SocketGuildUser user, string s) =>
            user.Roles.Any(x => x.Name == s);

        public static bool HasRole(this SocketGuildUser user, ulong u) =>
            user.Roles.Any(x => x.Id == u);

        public static bool HasPermission(this SocketGuildUser user, GuildPermission p) =>
            user.GuildPermissions.ToList().Contains(p);

        public static bool EnsureRank(this SocketGuildUser u, Server s, SocketGuild g)
        {
            if (!u.IsCreator())
            {
                if (!u.IsOwner(g))
                {
                    if (!u.HasAnyRole(s.Config.ModeratorRoles))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsOwner(this IUser u, IGuild g)
        {
            return u.Id == g.OwnerId;
        }
    }
}