using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Guild
    {
        ulong Id;
        ulong OwnerId;
        ulong Balance;
        ulong Exp;
        DateTime CreatedAt;
        List<GuildObjective> Objectives;
        List<GuildEvent> Events;
        List<GuildCommand> Commands;
        GuildConfig Config;
    }
}
