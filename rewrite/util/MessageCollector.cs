using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// A class that handles message collection within asynchronous tasks.
    /// </summary>
    public class MessageCollector<TUser> where TUser : IUser
    {

        private readonly AsyncEvent<Func<IUserMessage, Task>> _messageCollectedEvent = new AsyncEvent<Func<IUserMessage, Task>>();
        public event Func<IUserMessage, Task> MessageCollected
        {
            add => _messageCollectedEvent.Add(value);
            remove => _messageCollectedEvent.Remove(value);
        }

        public TUser User { get; }
        public List<IUserMessage> Messages { get; }
        public TimeSpan? Duration { get; }

        // public async Task StartAsync(); // begins collection messages
    }

    public class ReactionCollector<TMessage> where TMessage : IUserMessage
    {
        public ReactionCollector(TMessage message, string emoteId)
        {
            Message = message;
            EmoteId = emoteId;

        }

        public TMessage Message { get; }
        public string EmoteId { get; }
        public TimeSpan? Duration { get; }

        // public async Task StartAsync(); // begins collection messages
    }
}
