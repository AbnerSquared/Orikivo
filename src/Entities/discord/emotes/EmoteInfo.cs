using System;
using Discord;
using Discord.WebSocket;

namespace Orikivo
{

    public class EmoteInfo : EmoteBaseInfo
    {
        // RoleIds is when the guild emote is directly from twitch, which means that only the subs of that twitch can use it
        public EmoteInfo(GuildEmote guildEmote) : base(guildEmote)
        {
            CreatorId = guildEmote.CreatorId;
            Flags = new EmoteFlags(IsAnimated, guildEmote.RoleIds == null ? false : guildEmote.RoleIds.Count > 0, guildEmote.IsManaged, guildEmote.RequireColons); // GuildEmote.RequireColons?
        }
        // IsExclusive is referring to twitch-only emotes
        public ulong? CreatorId { get; } // null if integrated
        public EmoteFlags Flags { get; }
    }
}