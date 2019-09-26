using Discord;
using Discord.WebSocket;
using System;

namespace Orikivo
{
    // UserInfo // Basic user info if they can't be found in any connected guild.
    // UserDisplayInfo
    // UserGuildInfo // Pairs within UserDisplayInfo if the user is readable from a guild.
    // GuildUserDisplayInfo
    // RoleDisplayInfo
    // ChannelDisplayInfo
    // DiscordChannelType

    public class EntityInfo
    {
        internal EntityInfo(SocketGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            CreatedAt = guild.CreatedAt;
        }
        internal EntityInfo(IChannel channel)
        {
            Id = channel.Id;
            Name = channel.Name;
            CreatedAt = channel.CreatedAt;
        }

        internal EntityInfo(IUser user)
        {
            Id = user.Id;
            Name = user.Username;
            CreatedAt = user.CreatedAt;
        }

        internal EntityInfo(IRole role)
        {
            Id = role.Id;
            Name = role.Name;
            CreatedAt = role.CreatedAt;
        }

        internal EntityInfo(Emote emote)
        {
            Id = emote.Id;
            Name = emote.Name;
            CreatedAt = emote.CreatedAt;
        }

        public ulong Id { get; }
        public string Name { get; }

        public DateTimeOffset CreatedAt { get; }
    }
    }
