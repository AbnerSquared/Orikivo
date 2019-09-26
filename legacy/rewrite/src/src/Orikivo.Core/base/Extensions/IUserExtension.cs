using Discord;
using Discord.WebSocket;
using Orikivo.Storage;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public static class IUserExtension
    {
        private const ulong BOT_DEVELOPER = 181605794159001601;

        public static bool IsCreator(this IUser u)
        {
            return u.Id == BOT_DEVELOPER;
        }
        public static bool HasAnyRole(this SocketGuildUser u, params ulong[] roles)
            => HasAnyRole(u, roles.ToList());

        public static bool HasAnyRole(this SocketGuildUser u, List<ulong> roles)
        {
            foreach(ulong role in roles)
            {
                if (u.HasRole(role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}