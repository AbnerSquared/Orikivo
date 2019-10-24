using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    public class OriMessageInvoker : IDisposable
    {
        public BaseSocketClient RootClient { get; }
        // you want to transfer the discord client, command context, and the user who invoked it.
        public OriMessageInvoker(DiscordSocketClient client, OriMessageInvokerConfig invokerConfig = null)
        {
            RootClient = client;
            invokerConfig = invokerConfig ?? new OriMessageInvokerConfig();
            _defaultTimeout = invokerConfig.Timeout;
        }

        private TimeSpan _defaultTimeout { get; }
        public ulong MessageId { get; } // the base message id
        public ulong UserId { get; } // the user that invoked the listener
        public Task<SocketMessage> GetMessageAsync(OriCommandContext context, bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null, CancellationToken token = default)
        {
            MessageCriteria<SocketMessage> criterion = new MessageCriteria<SocketMessage>();
            if (fromSourceUser)
                criterion.AddCriterion(new SourceUserCriterion());
            if (inSourceChannel)
                criterion.AddCriterion(new SourceChannelCriterion());
            return GetMessageAsync(context, criterion, timeout, token);
        }
        // notice: you can create asynchronous tasks within async tasks
        public async Task<SocketMessage> GetMessageAsync(OriCommandContext context, IMessageCriterion<SocketMessage> criterion, TimeSpan? timeout = null, CancellationToken token = default)
        {
            timeout = timeout ?? _defaultTimeout;

            TaskCompletionSource<SocketMessage> eventTrigger = new TaskCompletionSource<SocketMessage>();
            TaskCompletionSource<bool> cancelTrigger = new TaskCompletionSource<bool>();

            token.Register(() => cancelTrigger.SetResult(true)); 
            // when needing to cancel, this sets the cancel trigger to true

            async Task Handler(SocketMessage message)
            {
                bool result = await criterion.JudgeAsync(context, message).ConfigureAwait(false);
                eventTrigger.SetResult(message);
            }

            context.Client.MessageReceived += Handler;
            Task<SocketMessage> trigger = eventTrigger.Task;
            Task<bool> cancel = cancelTrigger.Task;
            Task delay = Task.Delay(timeout.Value);
            Task task = await Task.WhenAny(trigger, delay, cancel).ConfigureAwait(false);
            context.Client.MessageReceived -= Handler;

            if (task == trigger)
                return await trigger.ConfigureAwait(false);
            else
                return null;
        }

        public void Dispose()
        {

        }
    }
}
