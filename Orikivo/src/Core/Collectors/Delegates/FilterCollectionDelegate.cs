using Discord.WebSocket;

namespace Orikivo
{
    /// <summary>
    /// Represents a message filter with inclusion to previously matched entries.
    /// </summary>
    /// <param name="message">The message that was read.</param>
    /// <param name="matches">A collection of all previous matches.</param>
    /// <param name="index">The current counter of messages handled.</param>
    public delegate bool FilterCollectionDelegate(SocketMessage message, MessageMatchCollection matches, int index);
}