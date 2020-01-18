using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Guild
    {
        public ulong Id { get; }

        public DateTime CreatedAt { get; }
        public ulong OwnerId { get; private set; }
        
        public ulong Balance { get; internal set; }
        public ulong Exp { get; internal set; }
        
        List<GuildObjective> Objectives;
        List<GuildEvent> Events;
        List<GuildCommand> Commands;
        GuildConfig Config;
    }
}
