using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    // Note use TriggerTypingAsync to let the user know that the broadcast is attempting to update.
    /// <summary>
    /// Represents a generic <see cref="IMessage"/> that other messages can be linked to.
    /// </summary>
    public interface ILinkedMessage
    {
        IMessage Source { get; }

        LinkDeleteHandling DeleteHandling { get; }

        IReadOnlyList<IUserMessage> Subscribers { get; }

        Task<IUserMessage> CreateAsync(IMessageChannel channel, RequestOptions options = null);

        Task<bool> AddAsync(IUserMessage message, RequestOptions options);

        bool Remove(IUserMessage message);

        Task DeleteAsync(RequestOptions options);
    }
}
