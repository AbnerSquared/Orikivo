using Discord;
using Discord.WebSocket;

namespace Orikivo
{
    // Barebones class
    public interface IDiscordEntity<T> where T : IEntity<ulong>
    {
        bool TryGetDiscordEntity(BaseSocketClient client, out T entity);
    }
}
