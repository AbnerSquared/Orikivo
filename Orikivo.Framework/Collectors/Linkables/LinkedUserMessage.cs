using Discord;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Represents a linkable message that can be modified.
    /// </summary>
    public class LinkedUserMessage : LinkedMessage, ILinkedMessage
    {
        protected LinkedUserMessage(IUserMessage source, LinkDeleteHandling handling) : base(source, handling)
        {
            Source = source;
        }

        public static LinkedUserMessage Create(IUserMessage message, LinkDeleteHandling handling)
            => new LinkedUserMessage(message, handling);

        public new IUserMessage Source { get; }
        IMessage ILinkedMessage.Source => Source;

        public async Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            await Source.ModifyAsync(func, options);
            await UpdateAsync(options);
        }
    }
}
