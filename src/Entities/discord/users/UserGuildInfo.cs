using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class UserGuildInfo
    {
        public UserGuildInfo(SocketGuildUser guildUser)
        {
            Guild = new GuildBaseInfo(guildUser.Guild);
            Nickname = guildUser.Nickname;
            JoinedAt = guildUser.JoinedAt;
            Permissions = guildUser.GuildPermissions;
            if (guildUser.VoiceState.HasValue)
                VoiceInfo = new UserVoiceInfo(guildUser.VoiceState.Value);
            List<RoleBaseInfo> roles = new List<RoleBaseInfo>();
            foreach (SocketRole role in guildUser.Roles)
                roles.Add(new RoleBaseInfo(role));
            Roles = roles;
            BoostingSince = guildUser.PremiumSince;
            Hierarchy = guildUser.Hierarchy;
        }

        // the basic role info
        public GuildBaseInfo Guild { get; }
        public string Nickname { get; }
        public DateTimeOffset? JoinedAt { get; }
        public DateTimeOffset? BoostingSince { get; }
        public int Hierarchy { get; }
        // generics
        public List<RoleBaseInfo> Roles { get; }
        public GuildPermissions Permissions { get; }
        public UserVoiceInfo VoiceInfo { get; }
    }
}