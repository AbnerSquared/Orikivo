using Discord;
using Discord.WebSocket;
using Orikivo.Storage;
using System;
using System.Linq;

namespace Orikivo
{
    public static class SocketRoleExtension
    {
        public static bool HasPermission(this SocketRole role, GuildPermission p) =>
            role.Permissions.ToList().Contains(p);
    }
}