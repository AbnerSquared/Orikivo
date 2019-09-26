using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class User
    {
        public User(OriUser user, params ulong[] guildIds)
        {
            Id = user.Id;
            Name = user.Username;
            GuildIds = guildIds.ToList();
            JoinedAt = DateTime.UtcNow;
        }

        public ulong Id { get; }
        public string Name { get; }

        public UserTag Tags { get; internal set; } // tags that define the general state of the user.
        public bool IsHost => Tags.HasFlag(UserTag.Host);
        public List<ulong> GuildIds { get; } = new List<ulong>();
        // this holds all guild ids that the user can interact in
        public ulong ReceiverId { get; } // this stores the main receiver that is used.
        // this handles excessive
        public DateTime? LastTrigger { get; internal set; } // when the last time the user executed a command was.
        public DateTime JoinedAt { get; } // when they joined the lobby
        // used to set the new host when the host leaves.

        public override string ToString()
            => $"{Name}{(IsHost ? " 🔱" : "")}";
    }
}
