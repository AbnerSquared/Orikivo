using Discord;
using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Orikivo.Messaging
{
    public class CooldownPrecondition : PreconditionAttribute
    {

        public TimeSpan Duration { get; set; } // however long the cooldown should be lasting
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());

        }
    }
}