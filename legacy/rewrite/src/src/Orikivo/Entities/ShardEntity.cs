using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Systems.Dependencies.Entities
{
    public static class ShardEntity
    {
        public static DiscordShardedClient GetDefault()
        {
            return new DiscordShardedClient
            {
                
            };
        }
    }
}
