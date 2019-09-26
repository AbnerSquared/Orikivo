using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace Orikivo.Dynamic
{
    public class DynamicManager : IDisposable
    {
        public BaseSocketClient Client { get; }
        private Dictionary<ulong, IReactionCallback> _callbacks;
        private TimeSpan _timeout;
        public DynamicManager(DiscordSocketClient client, TimeSpan? timeout = null)
        {
            Client = client;
            _timeout = timeout ?? TimeSpan.FromSeconds(20);
        }

        public async Task<SocketMessage> ListenToReplyAsync(OrikivoCommandContext Context, ICriteria<SocketMessage> criteria, TimeSpan? timeout = null)
        {
            timeout = timeout ?? _timeout;
            TaskCompletionSource<SocketMessage> trigger = new TaskCompletionSource<SocketMessage>();


            async Task Listener(SocketMessage message)
            {
                bool ensured = await criteria.EnsureAsync(Context, message).ConfigureAwait(false);
                if (ensured)
                    trigger.SetResult(message);
            }

            Context.Client.MessageReceived += Listener;

            Task<SocketMessage> handler = trigger.Task;
            Task delay = Task.Delay(timeout.Value);
            Task task = await Task.WhenAny(handler, delay).ConfigureAwait(false);

            Context.Client.MessageReceived -= Listener;

            if (task == handler)
                return await handler.ConfigureAwait(false);
            else
                return null;
        }

        public Task<SocketMessage> ListenToReplyAsync(OrikivoCommandContext Context, bool sourceUser = true, bool sourceChannel = true, TimeSpan? timeout = null)
        {
            Criteria<SocketMessage> criteria = new Criteria<SocketMessage>();

            if (sourceUser)
                criteria.Add(new EnsureUserCriteria());
            if (sourceChannel)
                criteria.Add(new EnsureChannelCriteria());

            return ListenToReplyAsync(Context, criteria, timeout);
        }

        public async Task<IUserMessage> ClearReplyAsync(OrikivoCommandContext Context, string content, bool tts = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
        {
            timeout = timeout ?? _timeout;
            IUserMessage message = await Context.Channel.SendMessageAsync(content, tts, embed, options).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value).ContinueWith(_ => message.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
            return message;
        }

        public async Task<IUserMessage> SendPaginatedMessageAsync(OrikivoCommandContext Context, PaginatedMessage message, ICriteria<SocketReaction> criteria = null)
        {
            PaginatedMessageCallback caller = new PaginatedMessageCallback(this, Context, message, criteria);
            await caller.ShowAsync().ConfigureAwait(false);
            return caller.Message;
        }

        public void AddReactionCallback(IMessage message, IReactionCallback callback)
            => _callbacks[message.Id] = callback;
        public void RemoveReactionCallback(IMessage message)
            => RemoveReactionCallback(message.Id);
        public void RemoveReactionCallback(ulong id)
            => _callbacks.Remove(id);
        public void ClearReactionCallbacks()
            => _callbacks.Clear();

        public async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == Client.CurrentUser.Id)
                return;
            if (!(_callbacks.TryGetValue(message.Id, out var callback)))
                return;
            if (!(await callback.Criteria.EnsureAsync(callback.Context, reaction).ConfigureAwait(false)))
                return;

            switch (callback.RunMode)
            {
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                            RemoveReactionCallback(message.Id);
                    });
                    break;
                default:
                    if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                        RemoveReactionCallback(message.Id);
                    break;
            }
        }

        public void Dispose()
        {
            Client.ReactionAdded -= HandleReactionAsync;
            // remove all handlers here.
        }
    }
}