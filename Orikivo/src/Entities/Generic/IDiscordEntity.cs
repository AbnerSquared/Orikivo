using Discord;
using Discord.WebSocket;

namespace Orikivo
{
    // Barebones class
    public interface IDiscordEntity<T>
        where T : IEntity<ulong>
    {

        bool GetEntity(BaseSocketClient client, out T entity);
    }
}
