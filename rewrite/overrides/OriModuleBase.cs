using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // based off of Discord.Addons.Interactive
    // extending modulebase to support dynamic events
    public abstract class OriModuleBase<T> : ModuleBase<T>
        where T : OriCommandContext
    {
        public OriMessageInvoker Invoker { get; set; }

        // a task that waits n seconds for a message, from which it returns.
        public Task<SocketMessage> GetMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null, CancellationToken token = default)
            => Invoker.GetMessageAsync(Context, criterion, timeout, token);
        public Task<SocketMessage> GetMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null, CancellationToken token = default)
            => Invoker.GetMessageAsync(Context, fromSourceUser, inSourceChannel, timeout, token);
    }
}
