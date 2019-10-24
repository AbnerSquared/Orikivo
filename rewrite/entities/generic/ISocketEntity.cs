﻿using Discord;
using Discord.WebSocket;

namespace Orikivo
{
    public interface IDiscordEntity<T> where T : IEntity<ulong>
    {
        bool TryGetSocketEntity(BaseSocketClient client, out T entity);
    }
}