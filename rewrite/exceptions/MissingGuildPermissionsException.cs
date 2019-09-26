using Discord;
using System;

namespace Orikivo
{
    public class MissingGuildPermissionsException : Exception
    {
        public MissingGuildPermissionsException(GuildPermission permission, ulong guildId, ActionType type)
        {
            Permission = permission;
            GuildId = guildId;
            Type = type;
        }
        // the permission that was required
        public GuildPermission Permission { get; }
        public ulong GuildId { get; }
        // the type of action that was about to occur
        public ActionType Type { get; }
        public override string Message => $"Orikivo is missing permissions to perform action '{Type.ToString()}' at {GuildId}: {Permission.ToString()}";
    }
}
