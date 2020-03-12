using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Represents a linkable message.
    /// </summary>
    public class LinkedMessage : ILinkedMessage
    {
        private readonly BaseSocketClient _client;

        protected LinkedMessage(IMessage source, LinkDeleteHandling handling, BaseSocketClient client = null)
        {
            Source = source;
            Id = source.Id;
            ChannelId = source.Channel.Id;
            DeleteHandling = handling;
            _subscribers = new List<IUserMessage>();

            if (client != null)
            {
                _client = client;
                _client.MessageUpdated += TryUpdateAsync;
                _client.MessageDeleted += TryDeleteAsync;
            }
        }


        ~LinkedMessage()
        {
            if (_client != null)
                if (!Disposed)
                    Unsubscribe();
        }

        private async Task TryUpdateAsync(Cacheable<IMessage, ulong> cached, SocketMessage message, ISocketMessageChannel channel)
        {
            if (message.Id != Source.Id || message.Channel.Id != Source.Channel.Id)
                return;

            await UpdateAsync();
        }

        public bool Disposed { get; protected set; } = false;

        private async Task TryDeleteAsync(Cacheable<IMessage, ulong> cached, ISocketMessageChannel channel)
        {
            if (!Remove(cached.Id))
                if (cached.HasValue)
                    if (cached.Value.Id == Id && cached.Value.Channel.Id == ChannelId)
                    {
                        if (DeleteHandling == LinkDeleteHandling.Source)
                            await MarkSubscribersAsync();
                        else
                            await DeleteSubscribersAsync();

                        if (_client != null)
                        {
                            Unsubscribe();
                        }
                    }
        }

        public static LinkedMessage Create(IMessage message, LinkDeleteHandling handling, BaseSocketClient client)
            => new LinkedMessage(message, handling, client);

        private static readonly string _onSourceDeleted = "[Original Message Deleted]";

        public IMessage Source { get; }

        public ulong Id { get; }
        protected ulong ChannelId { get; }

        public LinkDeleteHandling DeleteHandling { get; }

        protected List<IUserMessage> _subscribers;

        public IReadOnlyList<IUserMessage> Subscribers { get => _subscribers; }

        // creates a new linked message node 
        public async Task<IUserMessage> CreateAsync(IMessageChannel channel, RequestOptions options = null)
        {
            IUserMessage subscriber = await Source.CloneAsync(channel, options);
            
            _subscribers.Add(subscriber);

            return subscriber;
        }

        // this updates and synchronizes all messages.
        protected async Task UpdateAsync(RequestOptions options = null)
            => _subscribers.ForEach(async x => await UpdateSubscriberAsync(x, options));

        private async Task UpdateSubscriberAsync(IUserMessage message, RequestOptions options = null)
        {
            if (message == Source)
                return;

            MessageContent content = Source.Copy();
            await message.ModifyAsync(delegate (MessageProperties x)
            {
                x.Content = content?.Content ?? x.Content;
                x.Embed = content.Embed?.Build() ?? x.Embed;
            }, options);
        }

        /// <summary>
        /// Subscribes the specified <paramref name="message"/> to this message.
        /// </summary>
        public async Task<bool> AddAsync(IUserMessage message, RequestOptions options = null)
        {
            await UpdateSubscriberAsync(message, options);

            if (_subscribers.Contains(message))
                return false;

            _subscribers.Add(message);
            return true;
        }

        /// <summary>
        /// Unsubscribes the specified <paramref name="message"/> from this message.
        /// </summary>
        public bool Remove(IUserMessage message)
            => _subscribers.Remove(message);

        public bool Remove(ulong messageId)
            => _subscribers.Any(x => x.Id == messageId) ? _subscribers.RemoveAll(x => x.Id == messageId) > 0 : false;

        /// <summary>
        /// Deletes this message and all of its subscribers.
        /// </summary>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            if (Disposed)
                return;

            if (DeleteHandling == LinkDeleteHandling.Source)
                await DeleteSourceAsync(options);
            else
                await DeleteAllAsync(options);

            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (_client != null)
            {
                _client.MessageUpdated -= TryUpdateAsync;
                _client.MessageDeleted -= TryDeleteAsync;
                Disposed = true;
            }
        }

        private async Task DeleteSourceAsync(RequestOptions options = null)
        {
            await Source.DeleteAsync(options);
            await MarkSubscribersAsync(options);
        }

        private async Task MarkSubscribersAsync(RequestOptions options = null)
        {
            _subscribers.ForEach(async x => await x.ModifyAsync(y => y.Content = Check.NotNull(x.Content) ? MarkContent(x.Content) : _onSourceDeleted, options));
        }

        private async Task DeleteAllAsync(RequestOptions options = null)
        {
            await Source.DeleteAsync(options);
            await DeleteSubscribersAsync(options);
        }

        private async Task DeleteSubscribersAsync(RequestOptions options = null)
        {
            _subscribers.ForEach(async x => await x.DeleteAsync(options));
        }

        private string MarkContent(string content)
        {
            StringBuilder result = new StringBuilder();

            result.Append("~~");
            result.Append(content);
            result.AppendLine("~~");
            result.Append(_onSourceDeleted);

            return result.ToString();
        }
    }
}
