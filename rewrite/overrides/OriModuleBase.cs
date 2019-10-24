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

        // This sends a message, and then waits until any of the criteria are met, the timeout ends, or the token is called.
        public Task<SocketMessage> GetMessageAsync(IMessageCriterion<SocketMessage> criterion, TimeSpan? timeout = null, CancellationToken token = default)
            => Invoker.GetMessageAsync(Context, criterion, timeout, token);
        public Task<SocketMessage> GetMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null, CancellationToken token = default)
            => Invoker.GetMessageAsync(Context, fromSourceUser, inSourceChannel, timeout, token);
    }
}
