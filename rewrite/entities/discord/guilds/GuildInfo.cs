using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Orikivo
{
    public class GuildInfo : GuildBaseInfo
    {
        public GuildInfo(SocketGuild guild) : base(guild)
        {

        }

        public string SplashUrl { get; }
        public int UserCount { get; }
        
        // generics
        public List<ChannelBaseInfo> Channels { get; }
        public List<RoleBaseInfo> Roles { get; }
        public List<EmoteBaseInfo> Emotes { get; }

        // specifics
        public PremiumTier BoostTier { get; }
        public VerificationLevel VerificationLevel { get; }
        public string VoiceRegionId { get; }
    }
}