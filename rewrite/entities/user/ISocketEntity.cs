using Discord;
using Discord.WebSocket;

namespace Orikivo
{
    public interface ISocketEntity<T> where T : IEntity<ulong>
    {
        bool TryGetSocketEntity(BaseSocketClient client, out T entity);
    }
}
